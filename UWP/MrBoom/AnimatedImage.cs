// Copyright (c) Timofei Zhakov. All rights reserved.

using System.Collections.Generic;

namespace MrBoom
{
    public class AnimatedImage
    {
        private readonly Image[] images;

        public AnimatedImage(params Image[] images)
        {
            this.images = images;
        }

        public AnimatedImage(params AnimatedImage[] stripes)
        {
            int len = 0;

            foreach (AnimatedImage stripe in stripes)
            {
                len += stripe.Length;
            }

            List<Image> images = new List<Image>(len);

            foreach (AnimatedImage stripe in stripes)
            {
                images.AddRange(stripe.images);
            }

            this.images = images.ToArray();
        }

        public AnimatedImage(List<Image> images)
        {
            this.images = images.ToArray();
        }

        public Image this[int animateIndex]
        {
            get
            {
                return images[animateIndex % images.Length];
            }
        }

        public int Length { get => images.Length; }
    }
}
