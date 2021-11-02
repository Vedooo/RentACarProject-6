using Business.Abstract;
using Business.Constants.Message;
using Business.ValidationRules.FluentValidation;
using Core.Aspects.Autofac.Validation;
using Core.Utilities.Business;
using Core.Utilities.Results;
using Core.Utilities.Results.DataResultOptions.DataOption;
using Core.Utilities.Results.ResultOptions.Option;
using DataAccess.Abstract;
using Entity.Concrete;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Business.Concrete
{
    public class EfBrandManager : IBrandService
    {
        IBrandDal _brandDal;

        public EfBrandManager(IBrandDal brandDal)
        {
            _brandDal = brandDal;
        }

        [ValidationAspect(typeof(BrandValidator))]
        public IResult Add(Brand brand)
        {
            IResult result = BusinessRules.Run(CheckIfBrandNameAlreadyExists(brand.BrandName), CheckBrandExist(brand.BrandId));
            if (result != null)
            {
                return new ErrorResult(Messages.BrandNotAdded);
            }
            return new SuccessResult(Messages.BrandAdded);
        }

        public IResult Delete(Brand brand)
        {
            IResult result = BusinessRules.Run(CheckBrandExist(brand.BrandId));
            if (result != null)
            {
                return new ErrorResult();
            }
            return new SuccessResult(Messages.BrandDeleted);
        }

        public IDataResult<List<Brand>> GetAll()
        {
            if (DateTime.Now.Hour == 1)
            {
                return new ErrorDataResult<List<Brand>>(Messages.MaintainanceTimeBrand);
            }
            return new SuccessDataResult<List<Brand>>(Messages.BrandsListed);
        }

        public IDataResult<List<Brand>> GetById(int brandId)
        {
            return new SuccessDataResult<List<Brand>>(_brandDal.GetAll(b => b.BrandId == brandId));
        }

        [ValidationAspect(typeof(BrandValidator))]
        public IResult Update(Brand brand)
        {
            IResult result = BusinessRules.Run(CheckIfBrandNameAlreadyExists(brand.BrandName));
            if (result != null)
            {
                return new ErrorResult(Messages.BrandAlreadyExists);
            }
            _brandDal.Update(brand);
            return new SuccessResult(Messages.BrandUpdated);
        }


        //////////////////// Business Rules //////////////////////////////

        private IResult CheckIfBrandNameAlreadyExists(string brandName)
        {
            var result = _brandDal.GetAll(b => b.BrandName == brandName).Any();
            if (result == true)
            {
                return new SuccessResult(Messages.BrandAlreadyExists);
            }
            return new ErrorResult();
        }

        private IResult CheckBrandExist(int brandId)
        {
            var result = _brandDal.GetAll(b => b.BrandId == brandId).Any();
            if (!result)
            {
                return new ErrorResult(Messages.InvalidBrandName);
            }
            return new SuccessResult();
        }
    }
}
