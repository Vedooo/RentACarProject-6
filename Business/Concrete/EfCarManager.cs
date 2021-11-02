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
using Entity.Dto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Business.Concrete
{
    public class EfCarManager : ICarService
    {
        ICarDal _carDal;
        IBrandService _brandService;

        public EfCarManager(ICarDal carDal, IBrandService brandService)
        {
            _carDal = carDal;
            _brandService = brandService;
        }

        [ValidationAspect(typeof(CarValidator))]
        public IResult Add(Car car)
        {
            IResult result = BusinessRules.Run(ChectIfBrandLimitExceeded());
            if (result != null)
            {
                return result;
            }
            _carDal.Add(car);
            return new SuccessResult(Messages.CarAdded);
        }

        public IResult Delete(Car car)
        {
            IResult result = BusinessRules.Run(CheckCarIdExists(car.CarId));
            if (result != null)
            {
                return result;
            }
            _carDal.Delete(car);
            return new SuccessResult(Messages.CarDeleted);
        }

        public IDataResult<List<Car>> GetAll()
        {
            if (DateTime.Now.Hour == 21)
            {
                return new ErrorDataResult<List<Car>>(_carDal.GetAll(), Messages.MaintainanceTimeCar);
            }
            return new SuccessDataResult<List<Car>>(_carDal.GetAll(), Messages.CarsListed);
        }

        public IDataResult<List<Car>> GetByBrandId(int brandId)
        {
            return new SuccessDataResult<List<Car>>(_carDal.GetAll(c => c.BrandId == brandId), Messages.CarsListedByBrandId);
        }

        public IDataResult<List<Car>> GetByColorId(int colorId)
        {
            return new SuccessDataResult<List<Car>>(_carDal.GetAll(c => c.ColorId == colorId), Messages.CarsListedByColorId);
        }

        public IDataResult<List<Car>> GetByDailyPrice(decimal min, decimal max)
        {
            return new SuccessDataResult<List<Car>>(_carDal.GetAll(c => (c.DailyPrice >= min && c.DailyPrice <= max)), Messages.CarListedByDailyPrice);
        }

        public IDataResult<List<Car>> GetById(int carId)
        {
            return new SuccessDataResult<List<Car>>(_carDal.GetAll(c => c.CarId == carId));
        }

        public IDataResult<List<CarDetailDto>> GetCarDetail()
        {
            return new SuccessDataResult<List<CarDetailDto>>(_carDal.GetCarsDetails());
        }

        [ValidationAspect(typeof(CarValidator))]
        public IResult Update(Car car)
        {
            IResult result = BusinessRules.Run(CheckCarIdExists(car.CarId));
            if (result != null)
            {
                return new ErrorResult();
            }
            _carDal.Update(car);
            return new ErrorResult(Messages.CarInfoUpdated);
        }

        //////////////////// Business Rules //////////////////////////////

        private IResult ChectIfBrandLimitExceeded()
        {
            var result = _brandService.GetAll();
            if (result.Data.Count >= 15)
            {
                return new ErrorResult(Messages.BrandLimitExceeded);
            }
            return new SuccessResult();
        }


        private IResult CheckCarIdExists(int carId)
        {
            var result = _carDal.GetAll(c => c.CarId == carId).Count();
            if (result != null)
            {
                return new ErrorResult(Messages.CarIsAlreadyExist);
            }
            return new SuccessResult();
        }
    }
}
