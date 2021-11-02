using Core.Utilities.Results;
using Entity.Concrete;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Text;

namespace Business.Abstract
{
    public interface IImageService
    {
        IDataResult<List<Image>> GetAll();
        IDataResult<List<Image>> GetByCarId(int carId);
        IDataResult<Image> GetById(int id);
        IResult Add(IFormFile formFile, Image image);
        IResult Update(IFormFile formFile, Image image);
        IResult Delete(Image image);

    }
}
