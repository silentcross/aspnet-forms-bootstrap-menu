//-----------------------------------------------------------------------
// <copyright file="BootstrapMenu.cs">
//     Copyright (c) Jeremy Knight. All rights reserved.
//     This source is subject to The MIT License (MIT).
//     For more information, see https://github.com/knight0323/aspnet-forms-bootstrap-menu
// </copyright>
// <author>Jeremy Knight</author>
//-----------------------------------------------------------------------

using System;
using System.ComponentModel;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace JK.Bootstrap4Controls
{
    [ControlValueProperty("SelectedValue")]
    [DefaultEvent("MenuItemClick")]
    [SupportsEventValidation]
    [ToolboxData("<{0}:BootstrapMenu runat=\"server\"></{0}:BootstrapMenu>")]
    public sealed class Bootstrap4Menu : Menu
    {
        private const string hightlightActiveKey = "HighlightActive";

        /// <summary>
        /// Gets or sets the header text over the left list box.
        /// </summary>
        [Browsable(true)]
        [EditorBrowsable(EditorBrowsableState.Always)]
        [DefaultValue(false)]
        [DisplayName("HighlightActive")]
        public bool HighlightActive
        {
            get { return this.ViewState[hightlightActiveKey] != null && Convert.ToBoolean(this.ViewState[hightlightActiveKey]); }
            set { this.ViewState[hightlightActiveKey] = value; }
        }

        public override void RenderBeginTag(HtmlTextWriter writer)
        {
            // don't call base.RenderBeginTag()
        }

        public override void RenderEndTag(HtmlTextWriter writer)
        {
            // don't call base.RenderEndTag()
        }

        protected override void OnPreRender(EventArgs e)
        {
            // don't call base.OnPreRender(e);
            this.EnsureDataBound();
        }

        protected override void Render(HtmlTextWriter writer)
        {
            this.BuildItems(writer, this.Items, true);
        }

        protected override void EnsureDataBound()
        {
            base.EnsureDataBound();
        }

        private void BuildItems(HtmlTextWriter writer, MenuItemCollection items, bool isRoot = false, bool isDropDown = false)
        {
            if (items.Count <= 0)
            {
                return;
            }

            string cssClass = "nav-item";

            if (isRoot)
            {
                cssClass = "navbar-nav mr-auto mt-2 mt-lg-0";
                if (!string.IsNullOrEmpty(this.CssClass))
                {
                    cssClass += " " + this.CssClass;
                }
                writer.AddAttribute(HtmlTextWriterAttribute.Class, cssClass);
                writer.RenderBeginTag(HtmlTextWriterTag.Ul);
            }
            else
            {
                if (isDropDown)
                {
                    cssClass = "dropdown-menu";
                    if (!string.IsNullOrEmpty(this.CssClass))
                    {
                        cssClass += " " + this.CssClass;
                    }
                    writer.AddAttribute(HtmlTextWriterAttribute.Class, cssClass);
                    writer.RenderBeginTag(HtmlTextWriterTag.Div);
                }
                else
                {
                    writer.AddAttribute(HtmlTextWriterAttribute.Class, cssClass);
                    writer.RenderBeginTag(HtmlTextWriterTag.Li);
                }
            }

            foreach (MenuItem item in items)
            {
                this.BuildItem(writer, item, isDropDown);
            }

            writer.RenderEndTag(); // </ul>
        }

        private void BuildItem(HtmlTextWriter writer, MenuItem item, bool DropChild = false)
        {
            if (item == null)
            {
                throw new ArgumentNullException("item");
            }

            if (writer == null)
            {
                throw new ArgumentNullException("writer");
            }

            if (item.ChildItems.Count > 0)
            {
                writer.AddAttribute(HtmlTextWriterAttribute.Class, "nav-item dropdown");
            }
            else if (!DropChild)
            {
                writer.AddAttribute(HtmlTextWriterAttribute.Class, "nav-item");
            }

            if (this.IsLink(item))
            {
                if (!DropChild)
                {
                    writer.RenderBeginTag(HtmlTextWriterTag.Li);
                }
                this.RenderLink(writer, item, DropChild);
                if (!DropChild)
                {
                    writer.RenderEndTag(); // </li>
                }
            }
            else if (this.HasChildren(item))
            {
                writer.RenderBeginTag(HtmlTextWriterTag.Li);
                this.RenderDropDown(writer, item);
                writer.RenderEndTag(); // </li>
            }
            else
            {
                writer.RenderBeginTag(HtmlTextWriterTag.Li);
                //writer.RenderBeginTag(HtmlTextWriterTag.A);
                this.RenderLink(writer, item, DropChild);
                //writer.Write(item.Text);
                writer.RenderEndTag();
            }
        }

        private bool HasChildren(MenuItem item)
        {
            if (item == null)
            {
                throw new ArgumentNullException("item");
            }

            return item.ChildItems.Count > 0;
        }

        private bool IsLink(MenuItem item)
        {
            if (item == null)
            {
                throw new ArgumentNullException("item");
            }

            return item.Enabled && !string.IsNullOrEmpty(item.NavigateUrl);
        }

        private void RenderLink(HtmlTextWriter writer, MenuItem item, bool isDropDown = false)
        {
            if (item == null)
            {
                throw new ArgumentNullException("item");
            }

            if (writer == null)
            {
                throw new ArgumentNullException("writer");
            }
            var test = this.Page.ClientScript.GetPostBackClientHyperlink(
                this,
                "#" + item.ValuePath.Replace(this.PathSeparator.ToString(), "\\"),
                true);
            string href = !string.IsNullOrEmpty(item.NavigateUrl)
                ? this.Page.Server.HtmlEncode(this.ResolveClientUrl(item.NavigateUrl))
                : "#";
            writer.AddAttribute(HtmlTextWriterAttribute.Href, href);
            string cssClass = "";

            if (this.HighlightActive && this.Page.ResolveUrl(item.NavigateUrl) == this.Page.Request.Url.AbsolutePath)
            {
                cssClass += "active";
            }

            if (isDropDown)
            {
                cssClass += " dropdown-item";
            }
            else
            {
                cssClass += " nav-link";
            }
            writer.AddAttribute(HtmlTextWriterAttribute.Class, cssClass);
            string toolTip = !string.IsNullOrEmpty(item.ToolTip)
                ? item.ToolTip
                : item.Text;
            writer.AddAttribute(HtmlTextWriterAttribute.Title, toolTip);

            writer.RenderBeginTag(HtmlTextWriterTag.A);
            if (!string.IsNullOrWhiteSpace(item.Value))
            {
                writer.AddAttribute(HtmlTextWriterAttribute.Class, item.Value);
                writer.RenderBeginTag(HtmlTextWriterTag.I);
                writer.RenderEndTag(); // </i>
            }
            writer.Write(item.Text);
            writer.RenderEndTag(); // </a>
        }

        private void RenderDropDown(HtmlTextWriter writer, MenuItem item)
        {
            if (item == null)
            {
                throw new ArgumentNullException("item");
            }

            if (writer == null)
            {
                throw new ArgumentNullException("writer");
            }

            writer.AddAttribute(HtmlTextWriterAttribute.Href, "#");
            writer.AddAttribute(HtmlTextWriterAttribute.Class, "nav-link dropdown-toggle");
            writer.AddAttribute("aria-haspopup", "true");
            writer.AddAttribute("aria-expanded", "false");
            writer.AddAttribute("data-toggle", "dropdown");
            writer.RenderBeginTag(HtmlTextWriterTag.A);

            string anchorValue = item.Text + "&nbsp;";
            writer.Write(anchorValue);

            writer.AddAttribute(HtmlTextWriterAttribute.Class, "caret");
            writer.RenderBeginTag(HtmlTextWriterTag.B);
            writer.RenderEndTag(); // </b>          

            writer.RenderEndTag(); // </a>

            this.BuildItems(writer, item.ChildItems, false, true);
        }
    }
}
