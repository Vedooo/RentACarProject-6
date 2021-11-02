﻿using FluentValidation;
using System;
using System.Collections.Generic;
using System.Text;

namespace Core.CrossCuttingConcerns.AllValidation
{
    public static class ValidationTool
    {
        public static void Validate(IValidator validator, object entity)
        {
            var context = new ValidationContext<Object>(entity);
            var result = validator.Validate(context);
            if (!result.IsValid)
            {
                throw new ValidationException(result.Errors);
            }
        }
    }
}
