﻿using jQueryApi;
using jQueryApi.UI.Interactions;
using System;
using System.Collections;
using System.Runtime.CompilerServices;

namespace Serenity
{
    public class Flexify : Widget<FlexifyOptions>
    {
        private Int32 xDifference;
        private Int32 yDifference;

        public Flexify(jQueryObject container, FlexifyOptions options)
            : base(container, options)
        {
            var self = this;
            LazyLoadHelper.ExecuteOnceWhenShown(container, delegate {
                self.StoreInitialSize();
            });
        }

        private void StoreInitialSize()
        {
            if (Q.IsTrue(element.GetDataValue("flexify-init")))
                return;

            element.Data("flexify-width", options.DesignWidth ?? element.GetWidth());
            element.Data("flexify-height", options.DesignHeight ?? element.GetHeight());
            element.Data("flexify-init", true);

            var self = this;
            element.As<dynamic>().bind("resize", new Action<jQueryEvent, dynamic>(delegate(jQueryEvent e, dynamic ui)
            {
                self.ResizeElements();
            }));

            element.As<dynamic>().bind("resizestop", new Action<jQueryEvent, dynamic>(delegate(jQueryEvent e, dynamic ui)
            {
                self.ResizeElements();
            }));

            if (options.DesignWidth != null ||
                options.DesignHeight != null)
                self.ResizeElements();
        }

        private Double GetXFactor(jQueryObject element)
        {
            double? xFactor = null;
            if (options.GetXFactor != null)
                xFactor = options.GetXFactor(element);

            if (xFactor == null)
                xFactor = element.GetDataValue("flex-x").As<double?>();

            return xFactor ?? 1;
        }

        private Double GetYFactor(jQueryObject element)
        {
            double? yFactor = null;
            if (options.GetYFactor != null)
                yFactor = options.GetYFactor(element);

            if (yFactor == null)
                yFactor = element.GetDataValue("flex-y").As<double?>();

            return yFactor ?? 0;
        }

        private void ResizeElements()
        {
            var width = element.GetWidth();
            var initialWidth = element.GetDataValue("flexify-width").As<int?>();
            if (initialWidth == null)
            {
                element.Data("flexify-width", width);
                initialWidth = width;
            }

            var height = element.GetHeight();
            var initialHeight = element.GetDataValue("flexify-height").As<int?>();
            if (initialHeight == null)
            {
                element.Data("flexify-height", height);
                initialHeight = height;
            }

            xDifference = width - initialWidth.Value;
            yDifference = height - initialHeight.Value;

            var self = this;
            element.Find(".flexify").Each((i, e) => {
                self.ResizeElement(jQuery.FromElement(e));
            });
        }

        private void ResizeElement(jQueryObject element)
        {
            var xFactor = GetXFactor(element);
            if (xFactor != 0)
            {
                var initialWidth = element.GetDataValue("flexify-width").As<int?>();
                if (initialWidth == null)
                {
                    var width = element.GetWidth();
                    element.Data("flexify-width", width);
                    initialWidth = width;
                }

                element.Width(Math.Truncate(initialWidth.Value + xFactor * xDifference));
            }

            var yFactor = GetYFactor(element);
            if (yFactor != 0)
            {
                var initialHeight = element.GetDataValue("flexify-height").As<int?>();
                if (initialHeight == null)
                {
                    var height = element.GetHeight();
                    element.Data("flexify-height", height);
                    initialHeight = height;
                }

                element.Height(Math.Truncate(initialHeight.Value + yFactor * yDifference));
            }
        }
    }

    public static class FlexifyExtensions
    {
        public static jQueryObject FlexHeightOnly(this jQueryObject element, double flexY = 1)
        {
            return element.AddClass("flexify").Data("flex-y", flexY).Data("flex-x", 0);
        }
    }

    [Imported, Serializable]
    public class FlexifyOptions
    {
        public Func<jQueryObject, double?> GetXFactor { get; set; }
        public Func<jQueryObject, double?> GetYFactor { get; set; }
        public Int32? DesignWidth { get; set; }
        public Int32? DesignHeight { get; set; }
    }
}