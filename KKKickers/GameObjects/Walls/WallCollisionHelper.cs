using System.Drawing;
namespace KKKickers.GameObjects.Walls
{
    public static class WallCollisionHelper
    {
        public static void HandleWallCollision(Player player, RectangleF wallRect, bool isBounce = false)
        {
            var playerBounds = player.Bounds;
            if (playerBounds.Top > wallRect.Bottom - 10 && !isBounce)
            {
                player.BumpHead(wallRect);
            }
            else
            {
                float overlapLeft = playerBounds.Right - wallRect.Left;
                float overlapRight = wallRect.Right - playerBounds.Left;
                float overlapTop = playerBounds.Bottom - wallRect.Top + 24;
                float overlapBottom = wallRect.Bottom - playerBounds.Top + 3;

                float minOverlap = Math.Min(
                    Math.Min(overlapLeft, overlapRight),
                    Math.Min(overlapTop, overlapBottom)
                );
                if (minOverlap == overlapBottom)
                {
                    if (isBounce)
                    {
                        player.Position = new PointF(player.Position.X, wallRect.Bottom + 15);
                        player.ReverseJump();
                        isBounce = false;
                    }
                    else
                    {
                        player.VerticalVelocity = 0;
                        player.Position = new PointF(player.Position.X, wallRect.Bottom + 5);
                    }
                }
                else if (minOverlap == overlapTop)
                {
                    if (isBounce)
                    {
                        player.Jump();
                        return;
                    }
                    SlideThroughWall(player, wallRect);
                }
                else if (minOverlap == overlapRight)
                {
                    player.Position = new PointF(wallRect.Right - 1, player.Position.Y);
                    player.Slide();
                    if (!player.IsFacingRight) player.ReverseDirection();
                }
                else if (minOverlap == overlapLeft)
                {
                    player.Position = new PointF(wallRect.Left - playerBounds.Width + 1, player.Position.Y);
                    player.Slide();
                    if (player.IsFacingRight) player.ReverseDirection();
                }
                if (isBounce) player.Jump();
            }
        }

        private static void SlideThroughWall(Player player, RectangleF wallRect)
        {
            player.Position = new PointF(
                player.IsFacingRight ? wallRect.Right - 1 : wallRect.Left - player.Bounds.Width + 1,
                player.Position.Y + 10
            );
            player.Slide();
        }
    }
}