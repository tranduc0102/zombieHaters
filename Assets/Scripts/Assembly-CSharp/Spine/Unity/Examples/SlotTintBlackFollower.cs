using UnityEngine;

namespace Spine.Unity.Examples
{
	public class SlotTintBlackFollower : MonoBehaviour
	{
		[SpineSlot("", "", false, true, false)]
		[SerializeField]
		protected string slotName;

		[SerializeField]
		protected string colorPropertyName = "_Color";

		[SerializeField]
		protected string blackPropertyName = "_Black";

		public Slot slot;

		private MeshRenderer mr;

		private MaterialPropertyBlock mb;

		private int colorPropertyId;

		private int blackPropertyId;

		private void Start()
		{
			Initialize(false);
		}

		public void Initialize(bool overwrite)
		{
			if (overwrite || mb == null)
			{
				mb = new MaterialPropertyBlock();
				mr = GetComponent<MeshRenderer>();
				slot = GetComponent<ISkeletonComponent>().Skeleton.FindSlot(slotName);
				colorPropertyId = Shader.PropertyToID(colorPropertyName);
				blackPropertyId = Shader.PropertyToID(blackPropertyName);
			}
		}

		public void Update()
		{
			Slot slot = this.slot;
			if (slot != null)
			{
				mb.SetColor(colorPropertyId, slot.GetColor());
				mb.SetColor(blackPropertyId, slot.GetColorTintBlack());
				mr.SetPropertyBlock(mb);
			}
		}

		private void OnDisable()
		{
			mb.Clear();
			mr.SetPropertyBlock(mb);
		}
	}
}
