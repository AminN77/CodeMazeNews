using System.Collections.Generic;

namespace Entities.DataTransferObjects
{
    public class CategoryForUpdateDto
    {
        public string Title { get; set; }

        public IEnumerable<News> News { get; set; }
    }
}