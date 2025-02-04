namespace REStyles
{
	using UnityEngine;
	using UnityEngine.UI;

	public class StyleImage
    {
		[SerializeField]
		public Sprite Sprite;

		[SerializeField]
		public Color Color = Color.white;

		public virtual void ApplyTo(Image component)
		{
			if (component == null)
			{
				return;
			}

			component.sprite = Sprite;
			component.color = Color;

			component.SetAllDirty();
		}
	}
}

