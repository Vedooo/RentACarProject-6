﻿using Business.Abstract;
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
    public class EfColorManager : IColorService
    {
        IColorDal _colorDal;

        public EfColorManager(IColorDal colorDal)
        {
            _colorDal = colorDal;
        }

        [ValidationAspect(typeof(ColorValidator))]
        public IResult Add(Color color)
        {
            IResult result = BusinessRules.Run(CheckIfColorNameExists(color.ColorName), CheckColorExists(color.ColorId));
            if (result != null)
            {
                return new ErrorResult();
            }
            _colorDal.Add(color);
            return new SuccessResult(Messages.ColorAdded);
        }

        public IResult Delete(Color color)
        {
            _colorDal.Delete(color);
            return new SuccessResult(Messages.ColorDeleted);
        }

        public IDataResult<List<Color>> GetAll()
        {
            if (DateTime.Now.Hour == 1)
            {
                return new ErrorDataResult<List<Color>>(_colorDal.GetAll(), Messages.MaintainanceTimeColor);
            }
            return new SuccessDataResult<List<Color>>(_colorDal.GetAll(), Messages.ColorsListed);
        }

        public IDataResult<List<Color>> GetById(int colorId)
        {
            return new SuccessDataResult<List<Color>>(_colorDal.GetAll(co => co.ColorId == colorId), Messages.ColorsListedById);
        }

        [ValidationAspect(typeof(ColorValidator))]
        public IResult Update(Color color)
        {
            _colorDal.Update(color);
            return new SuccessResult(Messages.ColorUpdated);
        }

        //////////////////// Business Rules //////////////////////////////


        private IResult CheckIfColorNameExists(string colorName)
        {
            var result = _colorDal.GetAll(co => co.ColorName == colorName).Any();
            if (result)
            {
                return new ErrorResult();
            }
            return new SuccessResult();
        }

        private IResult CheckColorExists(int colorId)
        {
            var result = _colorDal.GetAll(co => co.ColorId == colorId).Any();
            if (!result)
            {
                return new ErrorResult();
            }
            return new SuccessResult();
        }
    }
}
