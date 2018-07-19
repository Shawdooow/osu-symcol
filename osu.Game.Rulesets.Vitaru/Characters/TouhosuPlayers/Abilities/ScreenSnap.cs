﻿using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using OpenTK.Graphics;
using osu.Framework.Allocation;
using osu.Framework.Configuration;
using osu.Framework.Graphics.Shapes;
using osu.Framework.Graphics.Textures;
using osu.Framework.IO.Stores;
using osu.Framework.Logging;
using osu.Framework.Platform;
using osu.Game.Configuration;
using Symcol.Core.Graphics.Sprites;
using Bitmap = System.Drawing.Bitmap;
// ReSharper disable InconsistentNaming

namespace osu.Game.Rulesets.Vitaru.Characters.TouhosuPlayers.Abilities
{
    public class ScreenSnap : SymcolSprite
    {
        private GameHost host;

        private readonly Box area;

        private static int img_count;
        private int imgCount;

        private static ResourceStore<byte[]> img_resources;
        private static TextureStore img_textures;

        public ScreenSnap(Box area)
        {
            Alpha = 0;
            AlwaysPresent = true;
            this.area = area;
        }

        [BackgroundDependencyLoader]
        private void load(OsuGame osu, OsuConfigManager config, GameHost host, Storage storage)
        {
            this.host = host;

#pragma warning disable 4014
            snap(storage, config.GetBindable<ScreenshotFormat>(OsuSetting.ScreenshotFormat));
#pragma warning restore 4014

        }

        public async Task snap(Storage storage, Bindable<ScreenshotFormat> screenshotFormat) => await Task.Run(async () =>
        {
            Rectangle rect = new Rectangle(new Point((int)area.DrawRectangle.Location.X, (int)area.DrawRectangle.Location.Y), new Size((int)area.DrawRectangle.Size.X, (int)area.DrawRectangle.Size.Y));

            using (var bitmap = await snapshot(rect))
            {
                switch (screenshotFormat.Value)
                {
                    case ScreenshotFormat.Png:
                        bitmap.Save(storage.GetStream("vitaru\\temp\\snapshot" + img_count + ".png", FileAccess.Write, FileMode.Create), ImageFormat.Png);
                        imgCount = img_count;
                        img_count++;
                        break;
                    case ScreenshotFormat.Jpg:
                        bitmap.Save(storage.GetStream("vitaru\\temp\\snapshot" + img_count + ".jpeg", FileAccess.Write, FileMode.Create), ImageFormat.Jpeg);
                        imgCount = img_count;
                        img_count++;
                        break;
                    default:
                        throw new ArgumentOutOfRangeException(nameof(screenshotFormat));
                }

                if (img_resources == null)
                {
                    img_resources = new ResourceStore<byte[]>(new StorageBackedResourceStore(storage.GetStorageForDirectory("vitaru\\temp")));
                    img_textures = new TextureStore(new RawTextureLoaderStore(img_resources));
                }
            }
        });

        protected override void LoadComplete()
        {
            base.LoadComplete();

            Texture = img_textures?.Get("snapshot" + imgCount + ".png") ?? img_textures?.Get("snapshot" + imgCount + ".jpeg");
        }

        /// <summary>
        /// FROM: osu.Framework.Platform.GameHost
        /// </summary>
        private async Task<Bitmap> snapshot(Rectangle rectangle)
        {
            Bitmap bitmap = new Bitmap(rectangle.Width, rectangle.Height);
            BitmapData data = bitmap.LockBits(rectangle, ImageLockMode.WriteOnly, PixelFormat.Format24bppRgb);

            bool complete = false;

            host.DrawThread.Scheduler.Add(() =>
            {
                if (GraphicsContext.CurrentContext == null)
                    throw new GraphicsContextMissingException();

                OpenTK.Graphics.OpenGL.GL.ReadPixels(rectangle.Location.X, rectangle.Location.Y, rectangle.Width, rectangle.Height, OpenTK.Graphics.OpenGL.PixelFormat.Bgr, OpenTK.Graphics.OpenGL.PixelType.UnsignedByte, data.Scan0);
                complete = true;
            });

            await Task.Run(() =>
            {
                while (!complete)
                    Thread.Sleep(50);
            });

            bitmap.UnlockBits(data);
            bitmap.RotateFlip(RotateFlipType.RotateNoneFlipY);

            return bitmap;
        }
    }
}
