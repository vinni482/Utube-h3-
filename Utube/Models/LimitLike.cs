using System;

namespace Utube.Models
{
    public class LimitLike
    {
        public int Id { get; set; }
        public Guid UserId { get; set; }
        public int VideoId { get; set; }

        public override bool Equals(object obj)
        {
            return ((LimitLike)obj).UserId == this.UserId && ((LimitLike)obj).VideoId == this.VideoId;
        }

        public override int GetHashCode()
        {
            return UserId.GetHashCode()+VideoId.GetHashCode();
        }
    }
}