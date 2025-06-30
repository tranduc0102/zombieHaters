using UnityEngine;

namespace Spine.Unity
{
	public class RegionlessAttachmentLoader : AttachmentLoader
	{
		private static AtlasRegion emptyRegion;

		private static AtlasRegion EmptyRegion
		{
			get
			{
				if (emptyRegion == null)
				{
					AtlasRegion atlasRegion = new AtlasRegion();
					atlasRegion.name = "Empty AtlasRegion";
					atlasRegion.page = new AtlasPage
					{
						name = "Empty AtlasPage",
						rendererObject = new Material(Shader.Find("Spine/Special/HiddenPass"))
						{
							name = "NoRender Material"
						}
					};
					emptyRegion = atlasRegion;
				}
				return emptyRegion;
			}
		}

		public RegionAttachment NewRegionAttachment(Skin skin, string name, string path)
		{
			RegionAttachment regionAttachment = new RegionAttachment(name);
			regionAttachment.RendererObject = EmptyRegion;
			return regionAttachment;
		}

		public MeshAttachment NewMeshAttachment(Skin skin, string name, string path)
		{
			MeshAttachment meshAttachment = new MeshAttachment(name);
			meshAttachment.RendererObject = EmptyRegion;
			return meshAttachment;
		}

		public BoundingBoxAttachment NewBoundingBoxAttachment(Skin skin, string name)
		{
			return new BoundingBoxAttachment(name);
		}

		public PathAttachment NewPathAttachment(Skin skin, string name)
		{
			return new PathAttachment(name);
		}

		public PointAttachment NewPointAttachment(Skin skin, string name)
		{
			return new PointAttachment(name);
		}

		public ClippingAttachment NewClippingAttachment(Skin skin, string name)
		{
			return new ClippingAttachment(name);
		}
	}
}
