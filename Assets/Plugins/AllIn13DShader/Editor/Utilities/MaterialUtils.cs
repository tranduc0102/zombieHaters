using System.Linq;
using UnityEditor;
using UnityEngine;

namespace AllIn13DShader
{
	public static class MaterialUtils
	{
		public const string STR_OUTLINE_TYPE_NONE = "_OUTLINETYPE_NONE";
		public const string STR_CAST_SHADOWS_ON = "_CAST_SHADOWS_ON";

		public static bool CheckMaterialShader(Material targetMat)
		{
			bool isOutline = !targetMat.shaderKeywords.Contains(STR_OUTLINE_TYPE_NONE);
			bool castShadowsEnabled = targetMat.shaderKeywords.Contains(STR_CAST_SHADOWS_ON);

			bool shaderChanged = false;

			Shader oldShader = null;
			Shader newShader = null;
			Shader shaderToCompare = null;

			if (isOutline)
			{

				if (castShadowsEnabled)
				{
					shaderToCompare = GlobalConfiguration.instance.shOutline;
				}
				else
				{
					shaderToCompare = GlobalConfiguration.instance.shOutlineNoShadowCaster;
				}
			}
			else
			{
				if (castShadowsEnabled)
				{
					shaderToCompare = GlobalConfiguration.instance.shStandard;
				}
				else
				{
					shaderToCompare = GlobalConfiguration.instance.shStandardNoShadowCaster;
				}
			}

			if (targetMat.shader != shaderToCompare)
			{
				oldShader = targetMat.shader;
				newShader = shaderToCompare;

				shaderChanged = true;
			}

			if (shaderChanged)
			{
				targetMat.shader = newShader;

				if (AssetDatabase.IsMainAsset(targetMat))
				{
					EditorUtility.SetDirty(targetMat);
				}
			}

			return shaderChanged;
		}
	}
}