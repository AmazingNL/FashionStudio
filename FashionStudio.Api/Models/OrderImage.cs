using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FashionStudio.Api.Models
{
    public class OrderImage
    {
        public OrderImage()
        {
        }
        //public OrderImage(int imageId, Order? order, User? user,
        //    WorkSpace? workSpace, string imageUrl, string description, string title)
        //{
        //    ImageId = imageId;
        //    Order = order;
        //    User = user;
        //    WorkSpace = workSpace;
        //    ImageUrl = imageUrl;
        //    Description = description;
        //    Title = title;
        //}
        [Key]
        public int Id { get; set; }

        public Order? Order { get; set; }
        public int OrderId { get; set; }

        public User? User { get; set; }
        public int UserId { get; set; }

        public WorkSpace? WorkSpace { get; set; }
        public int WorkSpaceId { get; set; }

        public string ImageUrl { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string Title { get; set; } = string.Empty;

    }
}

