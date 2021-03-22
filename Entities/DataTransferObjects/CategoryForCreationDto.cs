using System.Collections.Generic;

namespace Entities.DataTransferObjects
{
    public class CategoryForCreationDto
    {
        public string Title { get; set; }

        public IEnumerable<News> News { get; set; }
    }
}