using System.Collections.Generic;
using Gyldendal.Jini.SalesConfigurationServices.BusinessLayer;
using Gyldendal.Jini.SalesConfigurationServices.Common;
using Gyldendal.Jini.SalesConfigurationServices.Models;
using Gyldendal.Jini.SalesConfigurationServices.Models.Request;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Telerik.JustMock;

namespace Gyldendal.Jini.SalesConfigurationServices.Test
{
    [TestClass]
    public class JiniConfigurationServiceTest
    {
        [TestMethod]
        public void IsPublished()
        {
            var businessLayerFacade = Mock.Create<BusinessLayerFacade>();
            Mock.Arrange(() => businessLayerFacade.IsPublished("9788702056921", "http://localhost:31728/api/", "Gyldendal Uddannelse")).Returns(true);
            Assert.IsTrue(businessLayerFacade.IsPublished("9788702056921", "http://localhost:31728/api/", "Gyldendal Uddannelse"));

        }

        [TestMethod]
        public void TestHeleSkolen()
        {
            var businessLayerFacade = new BusinessLayerFacade();

            #region RequestObject
            var config = new PriceRequest()
            {
                SalesForms = new List<SalesForm>()
                {
                    new SalesForm()
                    {
                        PeriodTypeName = null,
                        Code = 1001,
                        DisplayName = "Abonnement"
                    }
                },
                Isbn = "9788702056921",
                AccessForms = new List<AccessForm>()
                {
                    new AccessForm()
                    {
                        PriceModels = new List<PriceModel>()
                        {
                            new PriceModel()
                            {
                                RefAccessFormCode = 1001,
                                ShowPercentage = false,
                                PercentageValue = 0,
                                GradeLevels = null,
                                Code = 1001,
                                DisplayName = "Hele Skolen"
                            }
                        },
                        BillingPeriods = new List<Period>()
                        {
                            new Period()
                            {
                                UnitValue = 6,
                                RefPeriodUnitTypeCode = 1002,
                                RefSalesFormCode = 0,
                                Id = 0,
                                Price = new Price()
                                {
                                    UnitPrice = 1,
                                    UnitPriceVat = (decimal) 1.25,
                                    VatValue = 25
                                },
                                Currency = "DK",
                                IsCustomPeriod = false,
                                Code = 0,
                                DisplayName = "6 måneders binding"
                            }
                        },
                        Code = 1001,
                        DisplayName = "Skole"
                    }
                }

            };
            #endregion RequestObject

            #region ExpectedObject
            var expected = new PriceRequest()
            {
                SalesForms = new List<SalesForm>()
                {
                    new SalesForm()
                    {
                        PeriodTypeName = null,
                        Code = 1001,
                        DisplayName = "Abonnement"
                    }
                },
                Isbn = "9788702056921",
                AccessForms = new List<AccessForm>()
                {
                    new AccessForm()
                    {
                        PriceModels = new List<PriceModel>()
                        {
                            new PriceModel()
                            {
                                RefAccessFormCode = 1001,
                                ShowPercentage = false,
                                PercentageValue = 0,
                                GradeLevels = null,
                                Code = 1001,
                                DisplayName = "Hele Skolen"
                            }
                        },
                        BillingPeriods = new List<Period>()
                        {
                            new Period()
                            {
                                UnitValue = 6,
                                RefPeriodUnitTypeCode = 1002,
                                RefSalesFormCode = 0,
                                Id = 0,
                                Price = new Price()
                                {
                                    UnitPrice = 1,
                                    CalculatedPrice = (decimal)1.25,
                                    UnitPriceVat = (decimal) 1.25,
                                    VatValue = 25
                                },
                                Currency = "DK",
                                IsCustomPeriod = false,
                                Code = 0,
                                DisplayName = "6 måneders binding"
                            }
                        },
                        Code = 1001,
                        DisplayName = "Skole"
                    }
                },

            };
            #endregion ExpectedObject


            config = businessLayerFacade.GetPrice("9788702056921", "335008",
                "https://test-gyldendalaccessservice.gyldendal.dk/api/", config);

            Assert.IsTrue(CustomAreEqual(config,expected));

        }

        [TestMethod]
        public void TesTeachertHeleSkolen()
        {
            var businessLayerFacade = new BusinessLayerFacade();

            #region RequestObject
            var config = new PriceRequest()
            {
                SalesForms = new List<SalesForm>()
                {
                    new SalesForm()
                    {
                        PeriodTypeName = null,
                        Code = 1001,
                        DisplayName = "Abonnement"
                    }
                },
                Isbn = "9788702056921",
                AccessForms = new List<AccessForm>()
                {
                    new AccessForm()
                    {
                        PriceModels = new List<PriceModel>()
                        {
                            new PriceModel()
                            {
                                RefAccessFormCode = 1003,
                                ShowPercentage = false,
                                PercentageValue = 0,
                                GradeLevels = null,
                                Code = 1008,
                                DisplayName = "Hele Skolen"
                            }
                        },
                        BillingPeriods = new List<Period>()
                        {
                            new Period()
                            {
                                UnitValue = 6,
                                RefPeriodUnitTypeCode = 1002,
                                RefSalesFormCode = 0,
                                Id = 0,
                                Price = new Price()
                                {
                                    UnitPrice = 1,
                                    UnitPriceVat = (decimal) 1.25,
                                    VatValue = 25
                                },
                                Currency = "DK",
                                IsCustomPeriod = false,
                                Code = 0,
                                DisplayName = "6 måneders binding"
                            }
                        },
                        Code = 1003,
                        DisplayName = "Underviser"
                    }
                }

            };
            #endregion RequestObject

            #region ExpectedObject
            var expected = new PriceRequest()
            {
                SalesForms = new List<SalesForm>()
                {
                    new SalesForm()
                    {
                        PeriodTypeName = null,
                        Code = 1001,
                        DisplayName = "Abonnement"
                    }
                },
                Isbn = "9788702056921",
                AccessForms = new List<AccessForm>()
                {
                    new AccessForm()
                    {
                        PriceModels = new List<PriceModel>()
                        {
                            new PriceModel()
                            {
                                RefAccessFormCode = 1003,
                                ShowPercentage = false,
                                PercentageValue = 0,
                                GradeLevels = null,
                                Code = 1008,
                                DisplayName = "Hele Skolen"
                            }
                        },
                        BillingPeriods = new List<Period>()
                        {
                            new Period()
                            {
                                UnitValue = 6,
                                RefPeriodUnitTypeCode = 1002,
                                RefSalesFormCode = 0,
                                Id = 0,
                                Price = new Price()
                                {
                                    UnitPrice = 1,
                                    CalculatedPrice = (decimal)1.25,
                                    UnitPriceVat = (decimal) 1.25,
                                    VatValue = 25
                                },
                                Currency = "DK",
                                IsCustomPeriod = false,
                                Code = 0,
                                DisplayName = "6 måneders binding"
                            }
                        },
                        Code = 1003,
                        DisplayName = "Underviser"
                    }
                },

            };
            #endregion ExpectedObject


            config = businessLayerFacade.GetPrice("9788702056921", "335008",
                "https://test-gyldendalaccessservice.gyldendal.dk/api/", config);

            Assert.IsTrue(CustomAreEqual(config, expected));

        }

        [TestMethod]
        public void TestNoOfStudents()
        {
            var businessLayerFacade = new BusinessLayerFacade();
            Mock.Arrange(() => Util.GetAsync<int>("https://test-gyldendalaccessservice.gyldendal.dk/api/", "v1/Unic/Institution/Students/Count/" + "335008")).Returns(2);

            #region RequestObject
            var config = new PriceRequest()
            {
                SalesForms = new List<SalesForm>()
                {
                    new SalesForm()
                    {
                        PeriodTypeName = null,
                        Code = 1001,
                        DisplayName = "Abonnement"
                    }
                },
                Isbn = "9788702056921",
                AccessForms = new List<AccessForm>()
                {
                    new AccessForm()
                    {
                        PriceModels = new List<PriceModel>()
                        {
                            new PriceModel()
                            {
                                RefAccessFormCode = 1001,
                                ShowPercentage = false,
                                PercentageValue = 0,
                                GradeLevels = null,
                                Code = 1002,
                                DisplayName = "Antal elever"
                            }
                        },
                        BillingPeriods = new List<Period>()
                        {
                            new Period()
                            {
                                UnitValue = 6,
                                RefPeriodUnitTypeCode = 1002,
                                RefSalesFormCode = 0,
                                Id = 0,
                                Price = new Price()
                                {
                                    UnitPrice = 1,
                                    UnitPriceVat = (decimal) 1.25,
                                    VatValue = 25
                                },
                                Currency = "DK",
                                IsCustomPeriod = false,
                                Code = 0,
                                DisplayName = "6 måneders binding"
                            }
                        },
                        Code = 1001,
                        DisplayName = "Skole"
                    }
                }

            };
            #endregion RequestObject

            #region ExpectedObject
            var expected = new PriceRequest()
            {
                SalesForms = new List<SalesForm>()
                {
                    new SalesForm()
                    {
                        PeriodTypeName = null,
                        Code = 1001,
                        DisplayName = "Abonnement"
                    }
                },
                Isbn = "9788702056921",
                AccessForms = new List<AccessForm>()
                {
                    new AccessForm()
                    {
                        PriceModels = new List<PriceModel>()
                        {
                            new PriceModel()
                            {
                                RefAccessFormCode = 1001,
                                ShowPercentage = false,
                                PercentageValue = 0,
                                GradeLevels = null,
                                Code = 1002,
                                DisplayName = "Antal elever"
                            }
                        },
                        BillingPeriods = new List<Period>()
                        {
                            new Period()
                            {
                                UnitValue = 6,
                                RefPeriodUnitTypeCode = 1002,
                                RefSalesFormCode = 0,
                                Id = 0,
                                Price = new Price()
                                {
                                    UnitPrice = 1,
                                    CalculatedPrice = (decimal)2.50,
                                    UnitPriceVat = (decimal) 1.25,
                                    VatValue = 25
                                },
                                Currency = "DK",
                                IsCustomPeriod = false,
                                Code = 0,
                                DisplayName = "6 måneders binding"
                            }
                        },
                        Code = 1001,
                        DisplayName = "Skole"
                    }
                },

            };
            #endregion ExpectedObject

            config = businessLayerFacade.GetPrice("9788702056921", "335008",
                "https://test-gyldendalaccessservice.gyldendal.dk/api/", config);

            Assert.IsTrue(CustomAreEqual(config, expected));
        }

        [TestMethod]
        public void TestPercentageOfStudents()
        {
            var businessLayerFacade = new BusinessLayerFacade();
            Mock.Arrange(() => Util.GetAsync<int>("https://test-gyldendalaccessservice.gyldendal.dk/api/", "v1/Unic/Institution/Students/Count/" + "335008")).Returns(100);

            #region RequestObject
            var config = new PriceRequest()
            {
                SalesForms = new List<SalesForm>()
                {
                    new SalesForm()
                    {
                        PeriodTypeName = null,
                        Code = 1001,
                        DisplayName = "Abonnement"
                    }
                },
                Isbn = "9788702056921",
                AccessForms = new List<AccessForm>()
                {
                    new AccessForm()
                    {
                        PriceModels = new List<PriceModel>()
                        {
                            new PriceModel()
                            {
                                RefAccessFormCode = 1001,
                                ShowPercentage = true,
                                PercentageValue = 2,
                                GradeLevels = null,
                                Code = 1003,
                                DisplayName = "Procentdel af elever"
                            }
                        },
                        BillingPeriods = new List<Period>()
                        {
                            new Period()
                            {
                                UnitValue = 6,
                                RefPeriodUnitTypeCode = 1002,
                                RefSalesFormCode = 0,
                                Id = 0,
                                Price = new Price()
                                {
                                    UnitPrice = 1,
                                    UnitPriceVat = (decimal) 1.25,
                                    VatValue = 25
                                },
                                Currency = "DK",
                                IsCustomPeriod = false,
                                Code = 0,
                                DisplayName = "6 måneders binding"
                            }
                        },
                        Code = 1001,
                        DisplayName = "Skole"
                    }
                }

            };
            #endregion RequestObject

            #region ExpectedObject
            var expected = new PriceRequest()
            {
                SalesForms = new List<SalesForm>()
                {
                    new SalesForm()
                    {
                        PeriodTypeName = null,
                        Code = 1001,
                        DisplayName = "Abonnement"
                    }
                },
                Isbn = "9788702056921",
                AccessForms = new List<AccessForm>()
                {
                    new AccessForm()
                    {
                        PriceModels = new List<PriceModel>()
                        {
                            new PriceModel()
                            {
                                RefAccessFormCode = 1001,
                                ShowPercentage = true,
                                PercentageValue = 2,
                                GradeLevels = null,
                                Code = 1003,
                                DisplayName = "Procentdel af elever"
                            }
                        },
                        BillingPeriods = new List<Period>()
                        {
                            new Period()
                            {
                                UnitValue = 6,
                                RefPeriodUnitTypeCode = 1002,
                                RefSalesFormCode = 0,
                                Id = 0,
                                Price = new Price()
                                {
                                    UnitPrice = 1,
                                    CalculatedPrice = (decimal)2.50,
                                    UnitPriceVat = (decimal) 1.25,
                                    VatValue = 25
                                },
                                Currency = "DK",
                                IsCustomPeriod = false,
                                Code = 0,
                                DisplayName = "6 måneders binding"
                            }
                        },
                        Code = 1001,
                        DisplayName = "Skole"
                    }
                },

            };
            #endregion ExpectedObject

            config = businessLayerFacade.GetPrice("9788702056921", "335008",
                "https://test-gyldendalaccessservice.gyldendal.dk/api/", config);

            Assert.IsTrue(CustomAreEqual(config, expected));
        }

        [TestMethod]
        public void TestNoOfStudentsInReleventClasses()
        {
            var businessLayerFacade = new BusinessLayerFacade();
            foreach (var grade in "2,3,4,5")
            {
                Mock.Arrange(() => Util.GetAsync<int>("https://test-gyldendalaccessservice.gyldendal.dk/api/", "v1/Unic/Institution/Students/Count/" + "335008" + "/GradeLevel/" + grade)).Returns(1);
            }
            
            #region RequestObject
            var config = new PriceRequest()
            {
                SalesForms = new List<SalesForm>()
                {
                    new SalesForm()
                    {
                        PeriodTypeName = null,
                        Code = 1001,
                        DisplayName = "Abonnement"
                    }
                },
                Isbn = "9788702056921",
                AccessForms = new List<AccessForm>()
                {
                    new AccessForm()
                    {
                        PriceModels = new List<PriceModel>()
                        {
                            new PriceModel()
                            {
                                RefAccessFormCode = 1001,
                                ShowPercentage = false,
                                PercentageValue = 0,
                                GradeLevels = "2,3,4,5",
                                Code = 1004,
                                DisplayName = "Antal elever på relevante klassetrin"
                            }
                        },
                        BillingPeriods = new List<Period>()
                        {
                            new Period()
                            {
                                UnitValue = 6,
                                RefPeriodUnitTypeCode = 1002,
                                RefSalesFormCode = 0,
                                Id = 0,
                                Price = new Price()
                                {
                                    UnitPrice = 1,
                                    UnitPriceVat = (decimal) 1.25,
                                    VatValue = 25
                                },
                                Currency = "DK",
                                IsCustomPeriod = false,
                                Code = 0,
                                DisplayName = "6 måneders binding"
                            }
                        },
                        Code = 1001,
                        DisplayName = "Skole"
                    }
                }

            };
            #endregion RequestObject

            #region ExpectedObject
            var expected = new PriceRequest()
            {
                SalesForms = new List<SalesForm>()
                {
                    new SalesForm()
                    {
                        PeriodTypeName = null,
                        Code = 1001,
                        DisplayName = "Abonnement"
                    }
                },
                Isbn = "9788702056921",
                AccessForms = new List<AccessForm>()
                {
                    new AccessForm()
                    {
                        PriceModels = new List<PriceModel>()
                        {
                            new PriceModel()
                            {
                                RefAccessFormCode = 1001,
                                ShowPercentage = false,
                                PercentageValue = 0,
                                GradeLevels = "2,3,4,5",
                                Code = 1004,
                                DisplayName = "Antal elever på relevante klassetrin"
                            }
                        },
                        BillingPeriods = new List<Period>()
                        {
                            new Period()
                            {
                                UnitValue = 6,
                                RefPeriodUnitTypeCode = 1002,
                                RefSalesFormCode = 0,
                                Id = 0,
                                Price = new Price()
                                {
                                    UnitPrice = 1,
                                    CalculatedPrice = (decimal)5.0,
                                    UnitPriceVat = (decimal) 1.25,
                                    VatValue = 25
                                },
                                Currency = "DK",
                                IsCustomPeriod = false,
                                Code = 0,
                                DisplayName = "6 måneders binding"
                            }
                        },
                        Code = 1001,
                        DisplayName = "Skole"
                    }
                },

            };
            #endregion ExpectedObject

            config = businessLayerFacade.GetPrice("9788702056921", "335008",
                "https://test-gyldendalaccessservice.gyldendal.dk/api/", config);

            Assert.IsTrue(CustomAreEqual(config, expected));
        }

        [TestMethod]
        public void TestNoOfReleventClasses()
        {
            var businessLayerFacade = new BusinessLayerFacade();
            foreach (var grade in "2,3,4,5")
            {
                Mock.Arrange(
                    () =>
                        Util.GetAsync<List<Class>>("https://test-gyldendalaccessservice.gyldendal.dk/api/",
                            "v1/Unic/Institution/Classes/" + 335008 + "/GradeLevel/" + grade)).Returns(
                                new List<Class>
                                {
                                    new Class
                                    {
                                        Name = "Klasse",
                                        StudentCount = 1,
                                    }
                                });
            }

            #region RequestObject
            var config = new PriceRequest()
            {
                SalesForms = new List<SalesForm>()
                {
                    new SalesForm()
                    {
                        PeriodTypeName = null,
                        Code = 1001,
                        DisplayName = "Abonnement"
                    }
                },
                Isbn = "9788702056921",
                AccessForms = new List<AccessForm>()
                {
                    new AccessForm()
                    {
                        PriceModels = new List<PriceModel>()
                        {
                            new PriceModel()
                            {
                                RefAccessFormCode = 1001,
                                ShowPercentage = false,
                                PercentageValue = 0,
                                GradeLevels = "2,3,4,5",
                                Code = 1005,
                                DisplayName = "Antal klasser på relevante klassetrin"
                            }
                        },
                        BillingPeriods = new List<Period>()
                        {
                            new Period()
                            {
                                UnitValue = 6,
                                RefPeriodUnitTypeCode = 1002,
                                RefSalesFormCode = 0,
                                Id = 0,
                                Price = new Price()
                                {
                                    UnitPrice = 1,
                                    UnitPriceVat = (decimal) 1.25,
                                    VatValue = 25
                                },
                                Currency = "DK",
                                IsCustomPeriod = false,
                                Code = 0,
                                DisplayName = "6 måneders binding"
                            }
                        },
                        Code = 1001,
                        DisplayName = "Skole"
                    }
                }

            };
            #endregion RequestObject

            #region ExpectedObject
            var expected = new PriceRequest()
            {
                SalesForms = new List<SalesForm>()
                {
                    new SalesForm()
                    {
                        PeriodTypeName = null,
                        Code = 1001,
                        DisplayName = "Abonnement"
                    }
                },
                Isbn = "9788702056921",
                AccessForms = new List<AccessForm>()
                {
                    new AccessForm()
                    {
                        PriceModels = new List<PriceModel>()
                        {
                            new PriceModel()
                            {
                                RefAccessFormCode = 1001,
                                ShowPercentage = false,
                                PercentageValue = 0,
                                GradeLevels = "2,3,4,5",
                                Code = 1005,
                                DisplayName = "Antal klasser på relevante klassetrin"
                            }
                        },
                        BillingPeriods = new List<Period>()
                        {
                            new Period()
                            {
                                UnitValue = 6,
                                RefPeriodUnitTypeCode = 1002,
                                RefSalesFormCode = 0,
                                Id = 0,
                                Price = new Price()
                                {
                                    UnitPrice = 1,
                                    CalculatedPrice = (decimal)5.0,
                                    UnitPriceVat = (decimal) 1.25,
                                    VatValue = 25
                                },
                                Currency = "DK",
                                IsCustomPeriod = false,
                                Code = 0,
                                DisplayName = "6 måneders binding"
                            }
                        },
                        Code = 1001,
                        DisplayName = "Skole"
                    }
                },

            };
            #endregion ExpectedObject

            config = businessLayerFacade.GetPrice("9788702056921", "335008",
                "https://test-gyldendalaccessservice.gyldendal.dk/api/", config);

            Assert.IsTrue(CustomAreEqual(config, expected));
        }

        [TestMethod]
        public void TestNoOfClasses()
        {
            var businessLayerFacade = new BusinessLayerFacade();
            
            #region RequestObject
            var config = new PriceRequest()
            {
                SalesForms = new List<SalesForm>()
                {
                    new SalesForm()
                    {
                        PeriodTypeName = null,
                        Code = 1001,
                        DisplayName = "Abonnement"
                    }
                },
                Isbn = "9788702056921",
                AccessForms = new List<AccessForm>()
                {
                    new AccessForm()
                    {
                        PriceModels = new List<PriceModel>()
                        {
                            new PriceModel()
                            {
                                RefAccessFormCode = 1002,
                                ShowPercentage = false,
                                PercentageValue = 0,
                                GradeLevels = "2,3,4,5",
                                Code = 1006,
                                DisplayName = "Antal klasser"
                            }
                        },
                        BillingPeriods = new List<Period>()
                        {
                            new Period()
                            {
                                UnitValue = 6,
                                RefPeriodUnitTypeCode = 1002,
                                RefSalesFormCode = 0,
                                Id = 0,
                                Price = new Price()
                                {
                                    UnitPrice = 1,
                                    UnitPriceVat = (decimal) 1.25,
                                    VatValue = 25
                                },
                                Currency = "DK",
                                IsCustomPeriod = false,
                                Code = 0,
                                DisplayName = "6 måneders binding"
                            }
                        },
                        Code = 1002,
                        DisplayName = "Klasse",
                        SelectedClasses = new List<Class>
                        {
                            new Class
                            {
                                Id = 2,
                                StudentCount = 1
                            },
                            new Class
                            {
                                Id = 3,
                                StudentCount = 2
                            }
                        }
                    }
                }

            };
            #endregion RequestObject

            #region ExpectedObject
            var expected = new PriceRequest()
            {
                SalesForms = new List<SalesForm>()
                {
                    new SalesForm()
                    {
                        PeriodTypeName = null,
                        Code = 1001,
                        DisplayName = "Abonnement"
                    }
                },
                Isbn = "9788702056921",
                AccessForms = new List<AccessForm>()
                {
                    new AccessForm()
                    {
                        PriceModels = new List<PriceModel>()
                        {
                            new PriceModel()
                            {
                                RefAccessFormCode = 1002,
                                ShowPercentage = false,
                                PercentageValue = 0,
                                GradeLevels = "2,3,4,5",
                                Code = 1006,
                                DisplayName = "Antal klasser"
                            }
                        },
                        BillingPeriods = new List<Period>()
                        {
                            new Period()
                            {
                                UnitValue = 6,
                                RefPeriodUnitTypeCode = 1002,
                                RefSalesFormCode = 0,
                                Id = 0,
                                Price = new Price()
                                {
                                    UnitPrice = 1,
                                    CalculatedPrice = (decimal)2.5,
                                    UnitPriceVat = (decimal) 1.25,
                                    VatValue = 25
                                },
                                Currency = "DK",
                                IsCustomPeriod = false,
                                Code = 0,
                                DisplayName = "6 måneders binding"
                            }
                        },
                        Code = 1002,
                        DisplayName = "Klasse",
                        SelectedClasses = new List<Class>
                        {
                            new Class
                            {
                                Id = 2,
                                StudentCount = 1
                            },
                            new Class
                            {
                                Id = 3,
                                StudentCount = 2
                            }
                        }
                    }
                },

            };
            #endregion ExpectedObject

            config = businessLayerFacade.GetPrice("9788702056921", "335008",
                "https://test-gyldendalaccessservice.gyldendal.dk/api/", config);

            Assert.IsTrue(CustomAreEqual(config, expected));
        }

        [TestMethod]
        public void TestNoOfStudentsInClasses()
        {
            var businessLayerFacade = new BusinessLayerFacade();
            var selectedClasses = new List<Class>
            {
                new Class
                {
                    Id = 2,
                    StudentCount = 1
                },
                new Class
                {
                    Id = 3,
                    StudentCount = 2
                }
            };
            foreach (var classObject in selectedClasses)
            {
                Mock.Arrange(
                    () =>
                        Util.GetAsync<int>("https://test-gyldendalaccessservice.gyldendal.dk/api/",
                            "v1/Unic/Institution/Students/Count/" + 335008 + "/Class/" + classObject.Name)).Returns(1);
            }

            #region RequestObject
            var config = new PriceRequest()
            {
                SalesForms = new List<SalesForm>()
                {
                    new SalesForm()
                    {
                        PeriodTypeName = null,
                        Code = 1001,
                        DisplayName = "Abonnement"
                    }
                },
                Isbn = "9788702056921",
                AccessForms = new List<AccessForm>()
                {
                    new AccessForm()
                    {
                        PriceModels = new List<PriceModel>()
                        {
                            new PriceModel()
                            {
                                RefAccessFormCode = 1002,
                                ShowPercentage = false,
                                PercentageValue = 0,
                                GradeLevels = "2,3,4,5",
                                Code = 1006,
                                DisplayName = "Antal klasser"
                            }
                        },
                        BillingPeriods = new List<Period>()
                        {
                            new Period()
                            {
                                UnitValue = 6,
                                RefPeriodUnitTypeCode = 1002,
                                RefSalesFormCode = 0,
                                Id = 0,
                                Price = new Price()
                                {
                                    UnitPrice = 1,
                                    UnitPriceVat = (decimal) 1.25,
                                    VatValue = 25
                                },
                                Currency = "DK",
                                IsCustomPeriod = false,
                                Code = 0,
                                DisplayName = "6 måneders binding"
                            }
                        },
                        Code = 1002,
                        DisplayName = "Klasse",
                        SelectedClasses = selectedClasses
                    }
                }

            };
            #endregion RequestObject

            #region ExpectedObject
            var expected = new PriceRequest()
            {
                SalesForms = new List<SalesForm>()
                {
                    new SalesForm()
                    {
                        PeriodTypeName = null,
                        Code = 1001,
                        DisplayName = "Abonnement"
                    }
                },
                Isbn = "9788702056921",
                AccessForms = new List<AccessForm>()
                {
                    new AccessForm()
                    {
                        PriceModels = new List<PriceModel>()
                        {
                            new PriceModel()
                            {
                                RefAccessFormCode = 1002,
                                ShowPercentage = false,
                                PercentageValue = 0,
                                GradeLevels = "2,3,4,5",
                                Code = 1006,
                                DisplayName = "Antal klasser"
                            }
                        },
                        BillingPeriods = new List<Period>()
                        {
                            new Period()
                            {
                                UnitValue = 6,
                                RefPeriodUnitTypeCode = 1002,
                                RefSalesFormCode = 0,
                                Id = 0,
                                Price = new Price()
                                {
                                    UnitPrice = 1,
                                    CalculatedPrice = (decimal)2.50,
                                    UnitPriceVat = (decimal) 1.25,
                                    VatValue = 25
                                },
                                Currency = "DK",
                                IsCustomPeriod = false,
                                Code = 0,
                                DisplayName = "6 måneders binding"
                            }
                        },
                        Code = 1002,
                        DisplayName = "Skole",
                        SelectedClasses = selectedClasses
                    }
                },

            };
            #endregion ExpectedObject

            config = businessLayerFacade.GetPrice("9788702056921", "335008",
                "https://test-gyldendalaccessservice.gyldendal.dk/api/", config);

            Assert.IsTrue(CustomAreEqual(config, expected));
        }

        [TestMethod]
        public void TestSingleUser()
        {
            var businessLayerFacade = new BusinessLayerFacade();
            #region RequestObject
            var config = new PriceRequest()
            {
                SalesForms = new List<SalesForm>()
                {
                    new SalesForm()
                    {
                        PeriodTypeName = null,
                        Code = 1001,
                        DisplayName = "Abonnement"
                    }
                },
                Isbn = "9788702056921",
                AccessForms = new List<AccessForm>()
                {
                    new AccessForm()
                    {
                        PriceModels = null,
                        BillingPeriods = new List<Period>()
                        {
                            new Period()
                            {
                                UnitValue = 6,
                                RefPeriodUnitTypeCode = 1002,
                                RefSalesFormCode = 0,
                                Id = 0,
                                Price = new Price()
                                {
                                    UnitPrice = 1,
                                    UnitPriceVat = (decimal) 1.25,
                                    VatValue = 25
                                },
                                Currency = "DK",
                                IsCustomPeriod = false,
                                Code = 0,
                                DisplayName = "6 måneders binding"
                            }
                        },
                        Code = 1004,
                        DisplayName = "Enkeltbruger",
                        StudentCount = 2
                    }
                }

            };
            #endregion RequestObject

            #region ExpectedObject
            var expected = new PriceRequest()
            {
                SalesForms = new List<SalesForm>()
                {
                    new SalesForm()
                    {
                        PeriodTypeName = null,
                        Code = 1001,
                        DisplayName = "Abonnement"
                    }
                },
                Isbn = "9788702056921",
                AccessForms = new List<AccessForm>()
                {
                    new AccessForm()
                    {
                        PriceModels = null,
                        BillingPeriods = new List<Period>()
                        {
                            new Period()
                            {
                                UnitValue = 6,
                                RefPeriodUnitTypeCode = 1002,
                                RefSalesFormCode = 0,
                                Id = 0,
                                Price = new Price()
                                {
                                    UnitPrice = 1,
                                    CalculatedPrice = (decimal)2.50,
                                    UnitPriceVat = (decimal) 1.25,
                                    VatValue = 25
                                },
                                Currency = "DK",
                                IsCustomPeriod = false,
                                Code = 0,
                                DisplayName = "6 måneders binding"
                            }
                        },
                        Code = 1004,
                        DisplayName = "Enkeltbruger",
                        StudentCount = 2
                    }
                },

            };
            #endregion ExpectedObject

            config = businessLayerFacade.GetPrice("9788702056921", "335008",
                "https://test-gyldendalaccessservice.gyldendal.dk/api/", config);

            Assert.IsTrue(CustomAreEqual(config, expected));
        }

        [TestMethod]
        public void GetPrice()
        {
            var businessLayerFacade = new BusinessLayerFacade();

            #region RequestObject
            var config = new PriceRequest()
            {
                SalesForms = new List<SalesForm>()
                {
                    new SalesForm()
                    {
                        PeriodTypeName = null,
                        Code = 1001,
                        DisplayName = "Abonnement"
                    }
                },
                Isbn = "9788702056921",
                AccessForms = new List<AccessForm>()
                {
                    new AccessForm()
                    {
                        PriceModels = new List<PriceModel>()
                        {
                            new PriceModel()
                            {
                                RefAccessFormCode = 1001,
                                ShowPercentage = false,
                                PercentageValue = 0,
                                GradeLevels = null,
                                Code = 1001,
                                DisplayName = "Hele Skolen"
                            }
                        },
                        BillingPeriods = new List<Period>()
                        {
                            new Period()
                            {
                                UnitValue = 6,
                                RefPeriodUnitTypeCode = 1002,
                                RefSalesFormCode = 0,
                                Id = 0,
                                Price = new Price()
                                {
                                    UnitPrice = 1,
                                    UnitPriceVat = (decimal) 1.25,
                                    VatValue = 25
                                },
                                Currency = "DK",
                                IsCustomPeriod = false,
                                Code = 0,
                                DisplayName = "6 måneders binding"
                            }
                        },
                        Code = 1001,
                        DisplayName = "Skole"
                    },
                    new AccessForm()
                    {
                        PriceModels = new List<PriceModel>()
                        {
                            new PriceModel()
                            {
                                RefAccessFormCode = 1003,
                                ShowPercentage = false,
                                PercentageValue = 0,
                                GradeLevels = null,
                                Code = 1008,
                                DisplayName = "Hele Skolen"
                            }
                        },
                        BillingPeriods = new List<Period>()
                        {
                            new Period()
                            {
                                UnitValue = 6,
                                RefPeriodUnitTypeCode = 1002,
                                RefSalesFormCode = 0,
                                Id = 0,
                                Price = new Price()
                                {
                                    UnitPrice = 1,
                                    UnitPriceVat = (decimal) 1.25,
                                    VatValue = 25
                                },
                                Currency = "DK",
                                IsCustomPeriod = false,
                                Code = 0,
                                DisplayName = "6 måneders binding"
                            }
                        },
                        Code = 1003,
                        DisplayName = "Underviser"
                    },
                    new AccessForm()
                    {
                        PriceModels = new List<PriceModel>()
                        {
                            new PriceModel()
                            {
                                RefAccessFormCode = 1002,
                                ShowPercentage = false,
                                PercentageValue = 0,
                                GradeLevels = "2,3,4,5",
                                Code = 1006,
                                DisplayName = "Antal klasser"
                            }
                        },
                        BillingPeriods = new List<Period>()
                        {
                            new Period()
                            {
                                UnitValue = 6,
                                RefPeriodUnitTypeCode = 1002,
                                RefSalesFormCode = 0,
                                Id = 0,
                                Price = new Price()
                                {
                                    UnitPrice = 1,
                                    UnitPriceVat = (decimal) 1.25,
                                    VatValue = 25
                                },
                                Currency = "DK",
                                IsCustomPeriod = false,
                                Code = 0,
                                DisplayName = "6 måneders binding"
                            }
                        },
                        Code = 1002,
                        DisplayName = "Klasse",
                        SelectedClasses = new List<Class>
                        {
                            new Class
                            {
                                Id = 2,
                                StudentCount = 1
                            },
                            new Class
                            {
                                Id = 3,
                                StudentCount = 2
                            }
                        }
                    },
                    new AccessForm()
                    {
                        PriceModels = null,
                        BillingPeriods = new List<Period>()
                        {
                            new Period()
                            {
                                UnitValue = 6,
                                RefPeriodUnitTypeCode = 1002,
                                RefSalesFormCode = 0,
                                Id = 0,
                                Price = new Price()
                                {
                                    UnitPrice = 1,
                                    UnitPriceVat = (decimal) 1.25,
                                    VatValue = 25
                                },
                                Currency = "DK",
                                IsCustomPeriod = false,
                                Code = 0,
                                DisplayName = "6 måneders binding"
                            }
                        },
                        Code = 1004,
                        StudentCount = 2,
                        DisplayName = "Enkeltbruger",

                    }
                }

            };
            #endregion RequestObject

            #region ExpectedObject
            var expected = new PriceRequest()
            {
                SalesForms = new List<SalesForm>()
                {
                    new SalesForm()
                    {
                        PeriodTypeName = null,
                        Code = 1001,
                        DisplayName = "Abonnement"
                    }
                },
                Isbn = "9788702056921",
                AccessForms = new List<AccessForm>()
                {
                    new AccessForm()
                    {
                        PriceModels = new List<PriceModel>()
                        {
                            new PriceModel()
                            {
                                RefAccessFormCode = 1001,
                                ShowPercentage = false,
                                PercentageValue = 0,
                                GradeLevels = null,
                                Code = 1001,
                                DisplayName = "Hele Skolen"
                            }
                        },
                        BillingPeriods = new List<Period>()
                        {
                            new Period()
                            {
                                UnitValue = 6,
                                RefPeriodUnitTypeCode = 1002,
                                RefSalesFormCode = 0,
                                Id = 0,
                                Price = new Price()
                                {
                                    UnitPrice = 1,
                                    CalculatedPrice = (decimal)1.25,
                                    UnitPriceVat = (decimal) 1.25,
                                    VatValue = 25
                                },
                                Currency = "DK",
                                IsCustomPeriod = false,
                                Code = 0,
                                DisplayName = "6 måneders binding"
                            }
                        },
                        Code = 1001,
                        DisplayName = "Skole"
                    },
                    new AccessForm()
                    {
                        PriceModels = new List<PriceModel>()
                        {
                            new PriceModel()
                            {
                                RefAccessFormCode = 1003,
                                ShowPercentage = false,
                                PercentageValue = 0,
                                GradeLevels = null,
                                Code = 1008,
                                DisplayName = "Hele Skolen"
                            }
                        },
                        BillingPeriods = new List<Period>()
                        {
                            new Period()
                            {
                                UnitValue = 6,
                                RefPeriodUnitTypeCode = 1002,
                                RefSalesFormCode = 0,
                                Id = 0,
                                Price = new Price()
                                {
                                    UnitPrice = 1,
                                    CalculatedPrice = (decimal)1.25,
                                    UnitPriceVat = (decimal) 1.25,
                                    VatValue = 25
                                },
                                Currency = "DK",
                                IsCustomPeriod = false,
                                Code = 0,
                                DisplayName = "6 måneders binding"
                            }
                        },
                        Code = 1003,
                        DisplayName = "Underviser"
                    },
                    new AccessForm()
                    {
                        PriceModels = new List<PriceModel>()
                        {
                            new PriceModel()
                            {
                                RefAccessFormCode = 1002,
                                ShowPercentage = false,
                                PercentageValue = 0,
                                GradeLevels = "2,3,4,5",
                                Code = 1006,
                                DisplayName = "Antal klasser"
                            }
                        },
                        BillingPeriods = new List<Period>()
                        {
                            new Period()
                            {
                                UnitValue = 6,
                                RefPeriodUnitTypeCode = 1002,
                                RefSalesFormCode = 0,
                                Id = 0,
                                Price = new Price()
                                {
                                    UnitPrice = 1,
                                    CalculatedPrice = (decimal)2.5,
                                    UnitPriceVat = (decimal) 1.25,
                                    VatValue = 25
                                },
                                Currency = "DK",
                                IsCustomPeriod = false,
                                Code = 0,
                                DisplayName = "6 måneders binding"
                            }
                        },
                        Code = 1002,
                        DisplayName = "Klasse",
                        SelectedClasses = new List<Class>
                        {
                            new Class
                            {
                                Id = 2,
                                StudentCount = 1
                            },
                            new Class
                            {
                                Id = 3,
                                StudentCount = 2
                            }
                        }
                    },
                    new AccessForm()
                    {
                        PriceModels = null,
                        BillingPeriods = new List<Period>()
                        {
                            new Period()
                            {
                                UnitValue = 6,
                                RefPeriodUnitTypeCode = 1002,
                                RefSalesFormCode = 0,
                                Id = 0,
                                Price = new Price()
                                {
                                    UnitPrice = 1,
                                    CalculatedPrice = (decimal)2.50,
                                    UnitPriceVat = (decimal) 1.25,
                                    VatValue = 25
                                },
                                Currency = "DK",
                                IsCustomPeriod = false,
                                Code = 0,
                                DisplayName = "6 måneders binding"
                            }
                        },
                        Code = 1004,
                        DisplayName = "Enkeltbruger",
                    }
                },

            };
            #endregion ExpectedObject


            config = businessLayerFacade.GetPrice("9788702056921", "335008",
                "https://test-gyldendalaccessservice.gyldendal.dk/api/", config);

            Assert.IsTrue(CustomAreEqual(config, expected));
        }

        //Util
        public bool CustomAreEqual(PriceRequest actual, PriceRequest expected)
        {
            if (actual.AccessForms.Count != expected.AccessForms.Count)
                return false;
            for (var i = 0; i < actual.AccessForms.Count; i++)
            {
                if (actual.AccessForms[i].BillingPeriods.Count != expected.AccessForms[i].BillingPeriods.Count)
                    return false;
                for (var j = 0; j < actual.AccessForms[i].BillingPeriods.Count; j++)
                {
                    if (actual.AccessForms[i].BillingPeriods[j].Price.CalculatedPrice !=
                        expected.AccessForms[i].BillingPeriods[j].Price.CalculatedPrice)
                        return false;
                }
            }
            return true;
        }
    }
}
