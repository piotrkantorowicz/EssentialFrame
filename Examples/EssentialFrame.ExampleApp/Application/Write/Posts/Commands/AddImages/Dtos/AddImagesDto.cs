using System.Collections.Generic;

namespace EssentialFrame.ExampleApp.Application.Write.Posts.Commands.AddImages.Dtos;

public class AddImagesDto
{
    public HashSet<ImageDto> Images { get; }
}