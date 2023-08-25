using System;

namespace EssentialFrame.ExampleApp.Application.Write.Posts.Dtos;

public class ImageDto
{
    public Guid ImageId { get; set; }

    public string ImageName { get; set; }

    public byte[] Bytes { get; set; }
}