namespace Microsoft.Maui
{
	public static class ILayoutExtensions
	{
		public static void InvalidateChildrenIsEnabled(this ILayout layout)
		{
			foreach (var children in layout.OrderByZIndex())
				children.Handler?.UpdateValue(nameof(IView.IsEnabled));
		}
	}
}