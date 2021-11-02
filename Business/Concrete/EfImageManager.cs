using Business.Abstract;
using Business.Constants.Message;
using Core.Utilities.Helper;
using Core.Utilities.Results;
using Core.Utilities.Results.DataResultOptions.DataOption;
using Core.Utilities.Results.ResultOptions.Option;
using DataAccess.Abstract;
using Entity.Concrete;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Business.Concrete
{
    public class EfImageManager : IImageService
    {
        IImageDal _imageDal;

        public EfImageManager(IImageDal imageDal)
        {
            _imageDal = imageDal;
        }

        public IResult Add(IFormFile formFile, Image image)
        {
            image.ImagePath = FileHelper.Add(formFile);
            image.Date = DateTime.UtcNow;
            _imageDal.Add(image);
            return new SuccessResult();
        }

        public IResult Delete(Image image)
        {
            FileHelper.Delete(_imageDal.Get(im => im.Id == image.Id).ImagePath);
            _imageDal.Delete(image);
            return new SuccessResult();
        }

        public IDataResult<List<Image>> GetAll()
        {
            return new SuccessDataResult<List<Image>>(_imageDal.GetAll(), Messages.ImagesListed);
        }

        public IDataResult<List<Image>> GetByCarId(int carId)
        {
            return new SuccessDataResult<List<Image>>(CheckNullImage(carId).Data);
        }

        public IDataResult<Image> GetById(int id)
        {
            return new SuccessDataResult<Image>(_imageDal.Get(im => im.Id == id));
        }

        public IResult Update(IFormFile formFile, Image image)
        {
            var oldPath = image.ImagePath;
            image.ImagePath = FileHelper.Update(_imageDal.Get(c => c.Id == image.Id).ImagePath, formFile);
            image.Date = DateTime.UtcNow;
            _imageDal.Update(image);
            return new SuccessResult();
        }


        //////////////////// Business Rules //////////////////////////////


        private IResult CheckImageLimit(int id)
        {
            var carImageCount = _imageDal.GetAll(c => c.CarId == id).Count;
            if (carImageCount > 5)
            {
                return new ErrorResult(Messages.ImageLimitExceed);
            }
            return new SuccessResult();
        }


        private IDataResult<List<Image>> CheckNullImage(int id)
        {
            try
            {
                string path = @"\images\default.jpg";
                var carImageCount = _imageDal.GetAll(c => c.CarId == id).Any();
                if (!carImageCount)
                {
                    List<Image> Image = new List<Image>();
                    Image.Add(new Image { CarId = id, ImagePath = path, Date = DateTime.Now });
                    return new SuccessDataResult<List<Image>>(Image);
                }
            }
            catch (Exception exception)
            {

                return new ErrorDataResult<List<Image>>(exception.Message);
            }
            return new SuccessDataResult<List<Image>>(_imageDal.GetAll(c => c.CarId == id));


        }
    }
}
