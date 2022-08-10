﻿#nullable enable
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using PlatformView = Microsoft.UI.Xaml.FrameworkElement;

namespace Microsoft.Maui.Handlers
{
	public partial class ViewHandler
	{
		partial void ConnectingHandler(PlatformView? platformView)
		{
			if (platformView != null)
			{
				platformView.GotFocus += OnPlatformViewGotFocus;
				platformView.LostFocus += OnPlatformViewLostFocus;
			}
		}

		partial void DisconnectingHandler(PlatformView platformView)
		{
			UpdateIsFocused(false);

			platformView.GotFocus -= OnPlatformViewGotFocus;
			platformView.LostFocus -= OnPlatformViewLostFocus;
		}

		static partial void MappingFrame(IViewHandler handler, IView view)
		{
			// Both Clip and Shadow depend on the Control size.
			handler.ToPlatform().UpdateClip(view);
			handler.ToPlatform().UpdateShadow(view);
		}

		public static void MapTranslationX(IViewHandler handler, IView view)
		{
			handler.ToPlatform().UpdateTransformation(view);
		}

		public static void MapTranslationY(IViewHandler handler, IView view)
		{
			handler.ToPlatform().UpdateTransformation(view);
		}

		public static void MapScale(IViewHandler handler, IView view)
		{
			handler.ToPlatform().UpdateTransformation(view);
		}

		public static void MapScaleX(IViewHandler handler, IView view)
		{
			handler.ToPlatform().UpdateTransformation(view);
		}

		public static void MapScaleY(IViewHandler handler, IView view)
		{
			handler.ToPlatform().UpdateTransformation(view);
		}

		public static void MapRotation(IViewHandler handler, IView view)
		{
			handler.ToPlatform().UpdateTransformation(view);
		}

		public static void MapRotationX(IViewHandler handler, IView view)
		{
			handler.ToPlatform().UpdateTransformation(view);
		}

		public static void MapRotationY(IViewHandler handler, IView view)
		{
			handler.ToPlatform().UpdateTransformation(view);
		}

		public static void MapAnchorX(IViewHandler handler, IView view)
		{
			handler.ToPlatform().UpdateTransformation(view);
		}

		public static void MapAnchorY(IViewHandler handler, IView view)
		{
			handler.ToPlatform().UpdateTransformation(view);
		}

		public static void MapToolbar(IViewHandler handler, IView view)
		{
			if (view is IToolbarElement tb)
				MapToolbar(handler, tb);
		}

		internal static void MapToolbar(IElementHandler handler, IToolbarElement toolbarElement)
		{
			_ = handler.MauiContext ?? throw new InvalidOperationException($"{nameof(handler.MauiContext)} null");

			if (toolbarElement.Toolbar != null)
			{
				var toolBar = toolbarElement.Toolbar.ToPlatform(handler.MauiContext);
				handler.MauiContext.GetNavigationRootManager().SetToolbar(toolBar);
			}
		}

		public static void MapContextFlyout(IViewHandler handler, IView view)
		{
			if (view is IContextFlyoutContainer contextFlyoutContainer)
			{
				MapContextFlyout(handler, contextFlyoutContainer);
			}
		}

		internal static void MapContextFlyout(IElementHandler handler, IContextFlyoutContainer contextFlyoutContainer)
		{
			_ = handler.MauiContext ?? throw new InvalidOperationException($"The handler's {nameof(handler.MauiContext)} cannot be null.");

			if (contextFlyoutContainer.ContextFlyout != null && contextFlyoutContainer.ContextFlyout.Any())
			{
				// REVIEW: I'd like to call this code, but it throws because the ContextFlyout doesn't yet have a
				// handler associated, so calling 'ToPlatform()' throws an exception. Instead, I've copied most of
				// the code of those code paths and changed it to make it work here to create the ContextFlyoutHandler.
				//var platformViewAttempt = contextFlyoutContainer.ContextFlyout.ToPlatform() ?? throw new InvalidOperationException($"Unable to convert view to {typeof(PlatformView)}");

				// This will set the MauiContext and get everything created first
				var handler2 = contextFlyoutContainer.ContextFlyout.ToHandler(handler.MauiContext);

				object? contextFlyoutPlatformView;
				if (contextFlyoutContainer.ContextFlyout is IReplaceableView replaceableView && replaceableView.ReplacedView != contextFlyoutContainer.ContextFlyout)
					contextFlyoutPlatformView = replaceableView.ReplacedView.ToPlatform();

				_ = contextFlyoutContainer.ContextFlyout.Handler ?? throw new InvalidOperationException($"{nameof(MauiContext)} should have been set on parent.");

				if (contextFlyoutContainer.ContextFlyout.Handler is IViewHandler viewHandler)
				{
					if (viewHandler.ContainerView is PlatformView containerView)
						contextFlyoutPlatformView = containerView;

					if (viewHandler.PlatformView is PlatformView platformView)
						contextFlyoutPlatformView = platformView;
				}

				contextFlyoutPlatformView = contextFlyoutContainer.ContextFlyout.Handler?.PlatformView;

				if (handler.PlatformView is Microsoft.UI.Xaml.UIElement uiElement && contextFlyoutPlatformView is FlyoutBase flyoutBase)
				{
					uiElement.ContextFlyout = flyoutBase;
				}
			}
		}

		public virtual bool NeedsContainer
		{
			get
			{
				if (VirtualView is IBorderView border)
					return border?.Shape != null || border?.Stroke != null;

				return false;
			}
		}

		void OnPlatformViewGotFocus(object sender, RoutedEventArgs args)
		{
			UpdateIsFocused(true);
		}

		void OnPlatformViewLostFocus(object sender, RoutedEventArgs args)
		{
			UpdateIsFocused(false);
		}

		void UpdateIsFocused(bool isFocused)
		{
			if (VirtualView == null)
				return;

			bool updateIsFocused = (isFocused && !VirtualView.IsFocused) || (!isFocused && VirtualView.IsFocused);

			if (updateIsFocused)
				VirtualView.IsFocused = isFocused;
		}
	}
}