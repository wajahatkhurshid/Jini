using System;
using System.Collections.Generic;
using Gyldendal.Jini.SalesConfigurationServices.BusinessLayer.Interfaces;
using Gyldendal.Jini.SalesConfigurationServices.Common;
using Gyldendal.Jini.SalesConfigurationServices.Models;
using Gyldendal.Jini.SalesConfigurationServices.Models.Request;

namespace Gyldendal.Jini.SalesConfigurationServices.BusinessLayer.Controllers
{
    public class PriceCalculator : IPriceCalculator
    {


        /// <summary>
        /// 
        /// </summary>
        /// <param name="isbn"></param>
        /// <param name="institutionNumber"></param>
        /// <param name="uniCServiceUrl"></param>
        /// <param name="salesConfiguration"></param>
        /// <returns>Returns populated Price Request object</returns>
        public PriceRequest GetPrice(string isbn, string institutionNumber, string uniCServiceUrl, PriceRequest salesConfiguration)
        {
            var result = salesConfiguration;
            //Foreach loop is read only. Need to update object
            // ReSharper disable once ForCanBeConvertedToForeach
            for (var i = 0; i < salesConfiguration.AccessForms.Count; i++)
            {
                var accessType = salesConfiguration.AccessForms[i];

                switch (accessType.Code)
                {
                    case (int)Enums.EnumAccessForm.SingleUser:
                        SingleUser(institutionNumber, accessType);
                        break;
                    case (int)Enums.EnumAccessForm.ContactSales:
                        break;
                    default:
                        foreach (var priceModel in accessType.PriceModels)
                        {
                            switch (priceModel.Code)
                            {
                                case (int)Enums.EnumPriceModel.WholeSchool:
                                    accessType = WholeSchool(institutionNumber, accessType);
                                    break;

                                case (int)Enums.EnumPriceModel.NoOfStudents:
                                    accessType = NoOfStudents(institutionNumber, accessType, uniCServiceUrl);
                                    break;

                                case (int)Enums.EnumPriceModel.PercentageOfStudents:
                                    accessType = PercentageOfStudents(institutionNumber, accessType, Convert.ToInt32(priceModel.PercentageValue), uniCServiceUrl);
                                    break;

                                case (int)Enums.EnumPriceModel.NoOfStudentsInReleventClasses:
                                    accessType = NoOfStudentsInReleventClasses(institutionNumber, accessType, priceModel.GradeLevels, uniCServiceUrl);
                                    break;

                                case (int)Enums.EnumPriceModel.NoOfReleventClasses:
                                    accessType = NoOfReleventClasses(institutionNumber, accessType, priceModel.GradeLevels, uniCServiceUrl);
                                    break;

                                case (int)Enums.EnumPriceModel.NoOfClasses:
                                    accessType = NoOfClasses(institutionNumber, accessType, accessType.SelectedClasses);
                                    break;

                                case (int)Enums.EnumPriceModel.NoOfStudentsInClasses:
                                    accessType = NoOfStudentsInClasses(institutionNumber, accessType, accessType.SelectedClasses, uniCServiceUrl);
                                    break;
                                case (int)Enums.EnumPriceModel.WholeSchoolTeacher:
                                    accessType = WholeSchool(institutionNumber, accessType);
                                    break;

                            }
                        }
                        break;
                }  
                
            }

            return result;
        }

        #region Calculate Price Per Price Model
        

        /// <summary>
        /// Complete school. Price is the same as Unit Price with Tax
        /// </summary>
        /// <param name="institutionNumber"></param>
        /// <param name="accessForm"></param>
        /// <returns>AccessForm</returns>
        public AccessForm WholeSchool(string institutionNumber, AccessForm accessForm)
        {
            foreach (var period in accessForm.BillingPeriods)
            {
                period.Price.CalculatedPrice = period.Price.UnitPriceVat;
            }
            return accessForm;
        }



        /// <summary>
        /// Student count for the institution is obtained from UniC service and price is calculated by the formula UnitPrice * Count
        /// </summary>
        /// <param name="institutionNumber"></param>
        /// <param name="accessForm"></param>
        /// <param name="uniCServiceUrl"></param>
        /// <returns>AccessForm</returns>
        public AccessForm NoOfStudents(string institutionNumber, AccessForm accessForm, string uniCServiceUrl)
        {
            int studentCount = Util.GetAsync<int>(uniCServiceUrl, "v1/Unic/Institution/Students/Count/" + institutionNumber);
            foreach (var period in accessForm.BillingPeriods)
            {
                period.Price.CalculatedPrice = period.Price.UnitPriceVat * studentCount;
            }
            return accessForm;
        }



        /// <summary>
        /// Student count for the institution is obtained from UniC service and price is calculated by the formula UnitPrice * ( Count * Percentage of Students)
        /// </summary>
        /// <param name="institutionNumber"></param>
        /// <param name="accessForm"></param>
        /// <param name="percentage"></param>
        /// <param name="uniCServiceUrl"></param>
        /// <returns>AccessForm</returns>
        public AccessForm PercentageOfStudents(string institutionNumber, AccessForm accessForm, int percentage, string uniCServiceUrl)
        {
            int studentCount = Util.GetAsync<int>(uniCServiceUrl, "v1/Unic/Institution/Students/Count/" + institutionNumber);
            foreach (var period in accessForm.BillingPeriods)
            {
                decimal percent = percentage;
                period.Price.CalculatedPrice = period.Price.UnitPriceVat * (studentCount * (percent / 100));
            }
            return accessForm;
        }



        /// <summary>
        /// 
        /// </summary>
        /// <param name="institutionNumber"></param>
        /// <param name="accessForm"></param>
        /// <param name="gradeLevels"></param>
        /// <param name="uniCServiceUrl"></param>
        /// <returns></returns>
        public AccessForm NoOfStudentsInReleventClasses(string institutionNumber, AccessForm accessForm, string gradeLevels, string uniCServiceUrl)
        {
            int totalStudents = 0;
            foreach (var grade in gradeLevels.Split(','))
            {
                totalStudents += Util.GetAsync<int>(uniCServiceUrl, "v1/Unic/Institution/Students/Count/" + institutionNumber + "/GradeLevel/" + grade);
            }

            foreach (var period in accessForm.BillingPeriods)
            {
                period.Price.CalculatedPrice = period.Price.UnitPriceVat * totalStudents;
            }

            return accessForm;
        }


        /// <summary>
        /// //
        /// </summary>
        /// <param name="institutionNumber"></param>
        /// <param name="accessForm"></param>
        /// <param name="gradeLevels"></param>
        /// <param name="uniCServiceUrl"></param>
        /// <returns></returns>
        public AccessForm NoOfReleventClasses(string institutionNumber, AccessForm accessForm, string gradeLevels, string uniCServiceUrl)
        {
            List<Class> classes = new List<Class>();
            foreach (var grade in gradeLevels.Split(','))
            {
                classes.AddRange(Util.GetAsync<List<Class>>(uniCServiceUrl, "v1/Unic/Institution/Classes/" + institutionNumber + "/GradeLevel/" + grade));
            }

            foreach (var period in accessForm.BillingPeriods)
            {
                period.Price.CalculatedPrice = period.Price.UnitPriceVat * classes.Count;
            }

            return accessForm;
        }



        /// <summary>
        /// 
        /// </summary>
        /// <param name="institutionNumber"></param>
        /// <param name="accessForm"></param>
        /// <param name="selectedClasses"></param>
        /// <returns></returns>
        public AccessForm NoOfClasses(string institutionNumber, AccessForm accessForm, List<Class> selectedClasses)
        {
            if (selectedClasses == null)
                throw new NullReferenceException();
            foreach (var period in accessForm.BillingPeriods)
            {
                period.Price.CalculatedPrice = period.Price.UnitPriceVat * selectedClasses.Count;
            }

            return accessForm;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="institutionNumber"></param>
        /// <param name="accessForm"></param>
        /// <param name="selectedClasses"></param>
        /// <param name="uniCServiceUrl"></param>
        /// <returns></returns>
        public AccessForm NoOfStudentsInClasses(string institutionNumber, AccessForm accessForm, List<Class> selectedClasses, string uniCServiceUrl)
        {
            int totalStudents = 0;
            if (selectedClasses == null)
                throw new NullReferenceException();
            foreach (var classObject in selectedClasses)
            {
                totalStudents += Util.GetAsync<int>(uniCServiceUrl, "v1/Unic/Institution/Students/Count/" + institutionNumber + "/Class/" + classObject.Name);
            }
            foreach (var period in accessForm.BillingPeriods)
            {
                period.Price.CalculatedPrice = period.Price.UnitPriceVat * totalStudents;
            }
            return accessForm;
        }


        

        /// <summary>
        /// 
        /// </summary>
        /// <param name="instituionNumber"></param>
        /// <param name="accessForm"></param>
        /// <returns></returns>
        public AccessForm SingleUser(string instituionNumber, AccessForm accessForm)
        {
            foreach (var period in accessForm.BillingPeriods)
            {
                if (accessForm.StudentCount == null)
                    throw new NullReferenceException();
                period.Price.CalculatedPrice = (int)accessForm.StudentCount * period.Price.UnitPriceVat;
            }
            return accessForm;
        }

        #endregion





    }
}
