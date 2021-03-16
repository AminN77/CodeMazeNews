﻿using System.Collections.Generic;
using Entities;

namespace Contracts
{
    public interface ICategoryRepository
    {
        IEnumerable<Category> GetAllCategories(bool trackChanges);
    }
}