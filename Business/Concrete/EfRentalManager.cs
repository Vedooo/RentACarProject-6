﻿using Business.Abstract;
using Business.Constants.Message;
using Business.ValidationRules.FluentValidation;
using Core.Aspects.Autofac.Validation;
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
    public class EfRentalManager : IRentalService
    {
        IRentalDal _rentalDal;

        public EfRentalManager(IRentalDal rentalDal)
        {
            _rentalDal = rentalDal;
        }

        [ValidationAspect(typeof(RentalValidator))]
        public IResult Add(Rental rental)
        {
            var result = IsCarAvaible(rental.CarId);
            if (result.Success)
            {
                _rentalDal.Add(rental);
                return new SuccessResult(Messages.RentalInfoAdded);
            }
            else
            {
                return new ErrorResult(result.Message);
            }

        }

        public IResult Delete(Rental rental)
        {
            _rentalDal.Delete(rental);
            return new SuccessResult(Messages.RentalInfoDeleted);
        }

        public IDataResult<List<Rental>> GetAll()
        {
            if (DateTime.Now.Hour == 1)
            {
                return new ErrorDataResult<List<Rental>>(_rentalDal.GetAll(), Messages.MaintainanceTimeRental);
            }
            return new SuccessDataResult<List<Rental>>(_rentalDal.GetAll(), Messages.RentalInfosListed);
        }

        public IDataResult<Rental> GetById(int rentId)
        {
            return new SuccessDataResult<Rental>(_rentalDal.Get(r => r.RentId == rentId));
        }

        public IDataResult<List<CarRentDetailDto>> GetCarDetail()
        {
            return new SuccessDataResult<List<CarRentDetailDto>>(_rentalDal.GetCarsDetails(), Messages.CarDetailsListed);
        }

        public IDataResult<List<CarRentDetailDto>> GetCarDetailById(int carId)
        {
            return new SuccessDataResult<List<CarRentDetailDto>>(_rentalDal.GetCarsDetailsById(carId), Messages.CarDetailsListedById);
        }

        public IResult IsCarAvaible(int carId)
        {
            if (IsCarAvaible(carId).Success)
            {
                if (IsCarReturned(carId).Success)
                {
                    return new SuccessResult(Messages.RentalInfoAdded);
                }
                return new ErrorResult(Messages.UnavaibleRentProcess);
            }
            return new SuccessResult(Messages.RentalInfoAdded);
        }

        public IResult IsCarEverRented(int carId)
        {
            if (_rentalDal.GetAll(r => r.CarId == carId).Any())
            {
                return new SuccessResult(Messages.CarRentedBefore);
            }
            return new ErrorResult(Messages.CarDidntRentBefore);
        }

        public IResult IsCarReturned(int carId)
        {
            if (_rentalDal.GetAll(r => (r.CarId == carId) && (r.ReturnDate == null)).Any())
            {
                return new ErrorResult(Messages.CarIsNotHere);
            }
            return new SuccessResult(Messages.CarIsHere);
        }

        [ValidationAspect(typeof(RentalValidator))]
        public IResult Update(Rental rental)
        {
            return new SuccessResult(Messages.RentProcessUpdated);
        }
    }
}
