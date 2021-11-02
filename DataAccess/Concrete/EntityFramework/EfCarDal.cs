using Core.DataAccess.EntityRepository;
using DataAccess.Abstract;
using Entity.Concrete;
using Entity.Dto;
using System.Linq;
using System;
using System.Collections.Generic;
using System.Text;

namespace DataAccess.Concrete.EntityFramework
{
    public class EfCarDal : EfEntityRepositoryBase<Car, CarContext>, ICarDal
    {
        public List<CarDetailDto> GetCarsDetails()
        {
            using (CarContext context = new CarContext())
            {
                var result = from c in context.Cars
                             join co in context.Colors on c.ColorId equals co.ColorId
                             join b in context.Brands on c.BrandId equals b.BrandId
                             select new CarDetailDto()
                             {
                                 CarId = c.CarId,
                                 BrandId = b.BrandId,
                                 BrandName = b.BrandName,
                                 ColorId = co.ColorId,
                                 ColorName = co.ColorName,
                                 ModelYear = c.ModelYear,
                                 DailyPrice = c.DailyPrice,
                                 Description = c.Description,
                             };
                return result.ToList();
            }
        }
    }
}
