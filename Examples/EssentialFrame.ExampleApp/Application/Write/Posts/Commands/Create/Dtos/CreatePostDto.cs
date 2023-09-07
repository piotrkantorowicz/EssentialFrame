using System;
using System.Collections.Generic;

namespace EssentialFrame.ExampleApp.Application.Write.Posts.Commands.Create.Dtos;

public class CreatePostDto
{
    public string Title { get; set; }

    public string Description { get; set; }

    public DateTimeOffset Expiration { get; set; }

    public HashSet<ImageDto> Images { get; set; } = new();
}