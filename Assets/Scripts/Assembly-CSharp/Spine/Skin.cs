using System;
using System.Collections.Generic;

namespace Spine
{
	public class Skin
	{
		public struct AttachmentKeyTuple
		{
			public readonly int slotIndex;

			public readonly string name;

			internal readonly int nameHashCode;

			public AttachmentKeyTuple(int slotIndex, string name)
			{
				this.slotIndex = slotIndex;
				this.name = name;
				nameHashCode = this.name.GetHashCode();
			}
		}

		private class AttachmentKeyTupleComparer : IEqualityComparer<AttachmentKeyTuple>
		{
			internal static readonly AttachmentKeyTupleComparer Instance = new AttachmentKeyTupleComparer();

			bool IEqualityComparer<AttachmentKeyTuple>.Equals(AttachmentKeyTuple o1, AttachmentKeyTuple o2)
			{
				return o1.slotIndex == o2.slotIndex && o1.nameHashCode == o2.nameHashCode && string.Equals(o1.name, o2.name, StringComparison.Ordinal);
			}

			int IEqualityComparer<AttachmentKeyTuple>.GetHashCode(AttachmentKeyTuple o)
			{
				return o.slotIndex;
			}
		}

		internal string name;

		private Dictionary<AttachmentKeyTuple, Attachment> attachments = new Dictionary<AttachmentKeyTuple, Attachment>(AttachmentKeyTupleComparer.Instance);

		public string Name
		{
			get
			{
				return name;
			}
		}

		public Dictionary<AttachmentKeyTuple, Attachment> Attachments
		{
			get
			{
				return attachments;
			}
		}

		public Skin(string name)
		{
			if (name == null)
			{
				throw new ArgumentNullException("name", "name cannot be null.");
			}
			this.name = name;
		}

		public void AddAttachment(int slotIndex, string name, Attachment attachment)
		{
			if (attachment == null)
			{
				throw new ArgumentNullException("attachment", "attachment cannot be null.");
			}
			attachments[new AttachmentKeyTuple(slotIndex, name)] = attachment;
		}

		public Attachment GetAttachment(int slotIndex, string name)
		{
			Attachment value;
			attachments.TryGetValue(new AttachmentKeyTuple(slotIndex, name), out value);
			return value;
		}

		public void FindNamesForSlot(int slotIndex, List<string> names)
		{
			if (names == null)
			{
				throw new ArgumentNullException("names", "names cannot be null.");
			}
			foreach (AttachmentKeyTuple key in attachments.Keys)
			{
				if (key.slotIndex == slotIndex)
				{
					names.Add(key.name);
				}
			}
		}

		public void FindAttachmentsForSlot(int slotIndex, List<Attachment> attachments)
		{
			if (attachments == null)
			{
				throw new ArgumentNullException("attachments", "attachments cannot be null.");
			}
			foreach (KeyValuePair<AttachmentKeyTuple, Attachment> attachment in this.attachments)
			{
				if (attachment.Key.slotIndex == slotIndex)
				{
					attachments.Add(attachment.Value);
				}
			}
		}

		public override string ToString()
		{
			return name;
		}

		internal void AttachAll(Skeleton skeleton, Skin oldSkin)
		{
			foreach (KeyValuePair<AttachmentKeyTuple, Attachment> attachment2 in oldSkin.attachments)
			{
				int slotIndex = attachment2.Key.slotIndex;
				Slot slot = skeleton.slots.Items[slotIndex];
				if (slot.Attachment == attachment2.Value)
				{
					Attachment attachment = GetAttachment(slotIndex, attachment2.Key.name);
					if (attachment != null)
					{
						slot.Attachment = attachment;
					}
				}
			}
		}
	}
}
