using System;

namespace Spine
{
	public struct BoneMatrix
	{
		public float a;

		public float b;

		public float c;

		public float d;

		public float x;

		public float y;

		public BoneMatrix(BoneData boneData)
		{
			float degrees = boneData.rotation + 90f + boneData.shearY;
			float degrees2 = boneData.rotation + boneData.shearX;
			a = MathUtils.CosDeg(degrees2) * boneData.scaleX;
			c = MathUtils.SinDeg(degrees2) * boneData.scaleX;
			b = MathUtils.CosDeg(degrees) * boneData.scaleY;
			d = MathUtils.SinDeg(degrees) * boneData.scaleY;
			x = boneData.x;
			y = boneData.y;
		}

		public BoneMatrix(Bone bone)
		{
			float degrees = bone.rotation + 90f + bone.shearY;
			float degrees2 = bone.rotation + bone.shearX;
			a = MathUtils.CosDeg(degrees2) * bone.scaleX;
			c = MathUtils.SinDeg(degrees2) * bone.scaleX;
			b = MathUtils.CosDeg(degrees) * bone.scaleY;
			d = MathUtils.SinDeg(degrees) * bone.scaleY;
			x = bone.x;
			y = bone.y;
		}

		public static BoneMatrix CalculateSetupWorld(BoneData boneData)
		{
			if (boneData == null)
			{
				return default(BoneMatrix);
			}
			if (boneData.parent == null)
			{
				return GetInheritedInternal(boneData, default(BoneMatrix));
			}
			BoneMatrix parentMatrix = CalculateSetupWorld(boneData.parent);
			return GetInheritedInternal(boneData, parentMatrix);
		}

		private static BoneMatrix GetInheritedInternal(BoneData boneData, BoneMatrix parentMatrix)
		{
			BoneData parent = boneData.parent;
			if (parent == null)
			{
				return new BoneMatrix(boneData);
			}
			float num = parentMatrix.a;
			float num2 = parentMatrix.b;
			float num3 = parentMatrix.c;
			float num4 = parentMatrix.d;
			BoneMatrix result = default(BoneMatrix);
			result.x = num * boneData.x + num2 * boneData.y + parentMatrix.x;
			result.y = num3 * boneData.x + num4 * boneData.y + parentMatrix.y;
			switch (boneData.transformMode)
			{
			case TransformMode.Normal:
			{
				float degrees3 = boneData.rotation + 90f + boneData.shearY;
				float num22 = MathUtils.CosDeg(boneData.rotation + boneData.shearX) * boneData.scaleX;
				float num23 = MathUtils.CosDeg(degrees3) * boneData.scaleY;
				float num24 = MathUtils.SinDeg(boneData.rotation + boneData.shearX) * boneData.scaleX;
				float num25 = MathUtils.SinDeg(degrees3) * boneData.scaleY;
				result.a = num * num22 + num2 * num24;
				result.b = num * num23 + num2 * num25;
				result.c = num3 * num22 + num4 * num24;
				result.d = num3 * num23 + num4 * num25;
				break;
			}
			case TransformMode.OnlyTranslation:
			{
				float degrees4 = boneData.rotation + 90f + boneData.shearY;
				result.a = MathUtils.CosDeg(boneData.rotation + boneData.shearX) * boneData.scaleX;
				result.b = MathUtils.CosDeg(degrees4) * boneData.scaleY;
				result.c = MathUtils.SinDeg(boneData.rotation + boneData.shearX) * boneData.scaleX;
				result.d = MathUtils.SinDeg(degrees4) * boneData.scaleY;
				break;
			}
			case TransformMode.NoRotationOrReflection:
			{
				float num16 = num * num + num3 * num3;
				float num17;
				if (num16 > 0.0001f)
				{
					num16 = Math.Abs(num * num4 - num2 * num3) / num16;
					num2 = num3 * num16;
					num4 = num * num16;
					num17 = MathUtils.Atan2(num3, num) * (180f / (float)Math.PI);
				}
				else
				{
					num = 0f;
					num3 = 0f;
					num17 = 90f - MathUtils.Atan2(num4, num2) * (180f / (float)Math.PI);
				}
				float degrees = boneData.rotation + boneData.shearX - num17;
				float degrees2 = boneData.rotation + boneData.shearY - num17 + 90f;
				float num18 = MathUtils.CosDeg(degrees) * boneData.scaleX;
				float num19 = MathUtils.CosDeg(degrees2) * boneData.scaleY;
				float num20 = MathUtils.SinDeg(degrees) * boneData.scaleX;
				float num21 = MathUtils.SinDeg(degrees2) * boneData.scaleY;
				result.a = num * num18 - num2 * num20;
				result.b = num * num19 - num2 * num21;
				result.c = num3 * num18 + num4 * num20;
				result.d = num3 * num19 + num4 * num21;
				break;
			}
			case TransformMode.NoScale:
			case TransformMode.NoScaleOrReflection:
			{
				float num5 = MathUtils.CosDeg(boneData.rotation);
				float num6 = MathUtils.SinDeg(boneData.rotation);
				float num7 = num * num5 + num2 * num6;
				float num8 = num3 * num5 + num4 * num6;
				float num9 = (float)Math.Sqrt(num7 * num7 + num8 * num8);
				if (num9 > 1E-05f)
				{
					num9 = 1f / num9;
				}
				num7 *= num9;
				num8 *= num9;
				num9 = (float)Math.Sqrt(num7 * num7 + num8 * num8);
				float radians = (float)Math.PI / 2f + MathUtils.Atan2(num8, num7);
				float num10 = MathUtils.Cos(radians) * num9;
				float num11 = MathUtils.Sin(radians) * num9;
				float num12 = MathUtils.CosDeg(boneData.shearX) * boneData.scaleX;
				float num13 = MathUtils.CosDeg(90f + boneData.shearY) * boneData.scaleY;
				float num14 = MathUtils.SinDeg(boneData.shearX) * boneData.scaleX;
				float num15 = MathUtils.SinDeg(90f + boneData.shearY) * boneData.scaleY;
				if (boneData.transformMode != TransformMode.NoScaleOrReflection && num * num4 - num2 * num3 < 0f)
				{
					num10 = 0f - num10;
					num11 = 0f - num11;
				}
				result.a = num7 * num12 + num10 * num14;
				result.b = num7 * num13 + num10 * num15;
				result.c = num8 * num12 + num11 * num14;
				result.d = num8 * num13 + num11 * num15;
				break;
			}
			}
			return result;
		}

		public BoneMatrix TransformMatrix(BoneMatrix local)
		{
			BoneMatrix result = default(BoneMatrix);
			result.a = a * local.a + b * local.c;
			result.b = a * local.b + b * local.d;
			result.c = c * local.a + d * local.c;
			result.d = c * local.b + d * local.d;
			result.x = a * local.x + b * local.y + x;
			result.y = c * local.x + d * local.y + y;
			return result;
		}
	}
}
