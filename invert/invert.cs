using HarmonyLib;
using OWML.Common;
using OWML.ModHelper;
using System.Reflection;

using UnityEngine;
using UnityEngine.UI.Extensions;

namespace invert;

[HarmonyPatch]
public class invert : ModBehaviour
{
	public static invert Instance;

	public void Awake()
	{
		Instance = this;
		// You won't be able to access OWML's mod helper in Awake.
		// So you probably don't want to do anything here.
		// Use Start() instead.
	}

	public void Start()
	{
		// Starting here, you'll have access to OWML's mod helper.
		ModHelper.Console.WriteLine($"{nameof(invert)} loaded", MessageType.Success);

		new Harmony("MrGruntie.InvertLines").PatchAll(Assembly.GetExecutingAssembly());
	}

    [HarmonyPrefix]
    [HarmonyPatch(typeof(LockOnReticule), nameof(LockOnReticule.SetMotionLines))]
    public static bool Reticle_Lock_Lines_Fix(LockOnReticule __instance, Vector2 relativeVel) {
		bool invert_x = Instance.ModHelper.Config.GetSettingsValue<bool>("Invert Target Velocity X Indicator");
		bool invert_y = Instance.ModHelper.Config.GetSettingsValue<bool>("Invert Target Velocity Y Indicator");
		SetMotionLinesFixed(relativeVel, invert_x, invert_y, __instance._lineX, __instance._lineY);
		return false;
    }

    public static void SetMotionLinesFixed(Vector2 relativeVel, bool invert_x, bool invert_y, UILineRenderer lineX, UILineRenderer lineY)
    {
		if (!invert_x) {
			relativeVel.x *= -1f;
		}
		if (!invert_y) {
			relativeVel.y *= -1f;
		}
        // relativeVel *= -1f;
        if ((bool)lineX)
        {
            lineX.gameObject.SetActive(value: true);
            lineX.Points[1] = ((relativeVel.x > 0f) ? new Vector2(45f, 0f) : new Vector2(-45f, 0f));
            lineX.Points[2] = lineX.Points[1] + new Vector2(relativeVel.x, 0f);
            lineX.uvRect = new Rect(0f, 0f, Mathf.Abs(relativeVel.x) / 60f, 1f);
            lineX.SetVerticesDirty();
            lineX.transform.GetChild(0).localPosition = lineX.Points[2];
            lineX.transform.GetChild(0).localEulerAngles = new Vector3((relativeVel.x > 0f) ? 180f : 0f, -90f, 90f);
        }
        if ((bool)lineY)
        {
            lineY.gameObject.SetActive(value: true);
            lineY.Points[1] = ((relativeVel.y > 0f) ? new Vector2(0f, 45f) : new Vector2(0f, -45f));
            lineY.Points[2] = lineY.Points[1] + new Vector2(0f, relativeVel.y);
            lineY.uvRect = new Rect(0f, 0f, Mathf.Abs(relativeVel.y) / 60f, 1f);
            lineY.SetVerticesDirty();
            lineY.transform.GetChild(0).localPosition = lineY.Points[2];
            lineY.transform.GetChild(0).localEulerAngles = new Vector3((relativeVel.y > 0f) ? (-90f) : 90f, -90f, 90f);
        }
    }

}

