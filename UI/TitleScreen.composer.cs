// AUTOMATICALLY GENERATED CODE

using System;
using System.Collections.Generic;
using Sce.Pss.Core;
using Sce.Pss.Core.Imaging;
using Sce.Pss.Core.Environment;
using Sce.Pss.HighLevel.UI;

namespace TTG12UI
{
    partial class TitleScreen
    {
        Panel sceneBackgroundPanel;
        Label Credits;
        Button startButton;
        ImageBox ImageBox_1;

        private void InitializeWidget()
        {
            InitializeWidget(LayoutOrientation.Horizontal);
        }

        private void InitializeWidget(LayoutOrientation orientation)
        {
            sceneBackgroundPanel = new Panel();
            sceneBackgroundPanel.Name = "sceneBackgroundPanel";
            Credits = new Label();
            Credits.Name = "Credits";
            startButton = new Button();
            startButton.Name = "startButton";
            ImageBox_1 = new ImageBox();
            ImageBox_1.Name = "ImageBox_1";

            // Credits
            Credits.TextColor = new UIColor(255f / 255f, 255f / 255f, 255f / 255f, 255f / 255f);
            Credits.Font = new Font( FontAlias.System, 25, FontStyle.Regular);
            Credits.LineBreak = LineBreak.Character;

            // startButton
            startButton.TextColor = new UIColor(0f / 255f, 0f / 255f, 0f / 255f, 255f / 255f);
            startButton.TextFont = new Font( FontAlias.System, 25, FontStyle.Regular);
            startButton.BackgroundFilterColor = new UIColor(255f / 255f, 255f / 255f, 255f / 255f, 255f / 255f);

            // ImageBox_1
            ImageBox_1.Image = new ImageAsset("assets/390988_10150451646196287_502726286_10901755_418795229_n.jpg");

            // Scene
            sceneBackgroundPanel.BackgroundColor = new UIColor(0f / 255f, 0f / 255f, 0f / 255f, 255f / 255f);

            this.RootWidget.AddChildLast(sceneBackgroundPanel);
            this.RootWidget.AddChildLast(Credits);
            this.RootWidget.AddChildLast(startButton);
            this.RootWidget.AddChildLast(ImageBox_1);

            SetWidgetLayout(orientation);

            UpdateLanguage();
        }

        private LayoutOrientation _currentLayoutOrientation;
        public void SetWidgetLayout(LayoutOrientation orientation)
        {
            switch (orientation)
            {
            case LayoutOrientation.Vertical:
                this.DesignWidth = 544;
                this.DesignHeight = 960;

                sceneBackgroundPanel.SetPosition(0, 0);
                sceneBackgroundPanel.SetSize(544, 960);
                sceneBackgroundPanel.Anchors = Anchors.Top | Anchors.Bottom | Anchors.Left | Anchors.Right;
                sceneBackgroundPanel.Visible = true;

                Credits.SetPosition(14, 498);
                Credits.SetSize(214, 36);
                Credits.Anchors = Anchors.Top | Anchors.Height | Anchors.Left | Anchors.Width;
                Credits.Visible = true;

                startButton.SetPosition(333, 198);
                startButton.SetSize(214, 56);
                startButton.Anchors = Anchors.Top | Anchors.Height | Anchors.Left | Anchors.Width;
                startButton.Visible = true;

                ImageBox_1.SetPosition(388, 148);
                ImageBox_1.SetSize(200, 200);
                ImageBox_1.Anchors = Anchors.Top | Anchors.Height | Anchors.Left | Anchors.Width;
                ImageBox_1.Visible = true;

                break;

            default:
                this.DesignWidth = 960;
                this.DesignHeight = 544;

                sceneBackgroundPanel.SetPosition(0, 0);
                sceneBackgroundPanel.SetSize(960, 544);
                sceneBackgroundPanel.Anchors = Anchors.Top | Anchors.Bottom | Anchors.Left | Anchors.Right;
                sceneBackgroundPanel.Visible = true;

                Credits.SetPosition(0, 297);
                Credits.SetSize(320, 93);
                Credits.Anchors = Anchors.Bottom | Anchors.Height | Anchors.Right | Anchors.Width;
                Credits.Visible = true;

                startButton.SetPosition(636, 54);
                startButton.SetSize(214, 56);
                startButton.Anchors = Anchors.Top | Anchors.Height | Anchors.Right | Anchors.Width;
                startButton.Visible = true;

                ImageBox_1.SetPosition(-10, 0);
                ImageBox_1.SetSize(538, 297);
                ImageBox_1.Anchors = Anchors.Top | Anchors.Height | Anchors.Left | Anchors.Width;
                ImageBox_1.Visible = true;

                break;
            }
            _currentLayoutOrientation = orientation;
        }
        public void UpdateLanguage()
        {
            Credits.Text = "Lindsay Cox, David Hart, Devon Hansen, Mike Bunby";
            startButton.Text = "Start";
            this.Title = "Title";
        }
        private void onShowing(object sender, EventArgs e)
        {
            switch (_currentLayoutOrientation)
            {
                case LayoutOrientation.Vertical:
                {
                }
                break;

                default:
                {
                }
                break;
            }
        }
        private void onShown(object sender, EventArgs e)
        {
            switch (_currentLayoutOrientation)
            {
                case LayoutOrientation.Vertical:
                {
                }
                break;

                default:
                {
                }
                break;
            }
        }
    }
}
