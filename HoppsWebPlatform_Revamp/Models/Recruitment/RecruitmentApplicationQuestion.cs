using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel;
using HoppsWebPlatform_Revamp.DataAccess;
using HoppsWebPlatform_Revamp.DataAccess.Interfaces;

namespace HoppsWebPlatform_Revamp.Models
{
    public class RecruitmentApplicationQuestion
    {
        [Key]
        [Required]
        public long ID { get; set; }
        //[Required]
        public string Description { get; set; }
        public string DataType { get; set; }
        [ValidateDataType]
        [Required]
        public string Answer { get; set; }
        public bool Active { get; set; }
    }

    /// <summary>
    /// Class for recruit app answer conversion to generic, possibly allow this to be used accross models by use of property names as parameters.
    /// </summary>
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
    public class ValidateDataType : ValidationAttribute
    {
        private IRecruitmentRepository _recruitmentRepository;

        public ValidateDataType()
        {
            _recruitmentRepository = new RecruitmentRepository();
        }

        /// <summary>
        /// Override for IsValid to determin whether the answer value is of the correct datatype specified by the datatype property. - 
        /// Note - Datatype value does not need to be case sensitive
        /// </summary>
        /// <param name="value">Value attached to attribute</param>
        /// <param name="validationContext">Context of which the property sits.</param>
        /// <returns>Validation result based on the conversion</returns>
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            object answer;
            object dataType;
            try
            {
                //Gets the values of the properties in the current model               
                answer = validationContext.ObjectType.GetProperty("Answer").GetValue(validationContext.ObjectInstance, null);
                //object dataType = validationContext.ObjectType.GetProperty("DataType").GetValue(validationContext.ObjectInstance,null);
                dataType = _recruitmentRepository.GetQuestionByQuestionID(Convert.ToInt64(validationContext.ObjectType.GetProperty("ID").GetValue(validationContext.ObjectInstance, null).ToString())).DataType;
                string dataTypeTitleCase = new System.Globalization.CultureInfo("en-US", false).TextInfo.ToTitleCase(dataType.ToString());

                if (value != null)
                {
                    //Uses string value to determin base value type for answer - Must be base value type to invoke GetConverter.
                    Type t = Type.GetType("System." + dataTypeTitleCase);

                    try
                    {
                        //Attempt to convert from string to generic base type value
                        object sss = TypeDescriptor.GetConverter(t).ConvertFromInvariantString(answer.ToString());
                        //If the conversion was successfull, let them know!!
                        return ValidationResult.Success;
                    }
                    catch (Exception)
                    {
                        string dataTypeSimpleName = "_DATA_TYPE_";
                        switch (dataTypeTitleCase.ToUpper())
                        {
                            case "STRING":
                                dataTypeSimpleName = "Text";
                                break;
                            case "INT32":
                                dataTypeSimpleName = "Number";
                                break;
                            case "INT64":
                                dataTypeSimpleName = "Number";
                                break;
                        }
                        //If the conversion failed or something else, then proboly shouldnt say that it is valid.
                        return new ValidationResult("Invalid data, must be of type : " + dataTypeSimpleName);
                    }
                }
                //Value is empty.
                return new ValidationResult("Value is required.");
            }
            catch (Exception)
            {
                return new ValidationResult("Error Retrieving Question ID");
            }

        }
    }
}