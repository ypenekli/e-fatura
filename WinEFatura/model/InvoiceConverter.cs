using System;
using System.Collections.Generic;
using System.Text;
using com.yp.efatura.data;
using System.IO;
using System.Linq;

namespace com.yp.efatura.model
{
    public class InvoiceConverter : IInvoiceConverter<InvoiceType, vkFatura>
    {

       public InvoiceType ToInvoice(vkFatura dataSource, String attachment)
        {
            vkFatura.faturalarRow fatura = dataSource.faturalar.First();
            vkFatura.faturaiciDataTable faturaici = dataSource.faturaici;
            var invoice = new InvoiceType {
                
                UBLVersionID = new UBLVersionIDType { Value = "2.1" },
                CustomizationID = new CustomizationIDType { Value = "TR1.2" },
                ProfileID = new ProfileIDType { Value = "EARSIVFATURA" },
                ID = new IDType { Value = fatura.faturanu },
                CopyIndicator = new CopyIndicatorType { Value = false },
                UUID = new UUIDType { Value = Guid.NewGuid().ToString() },
                IssueDate = new IssueDateType { Value = fatura.tarih },
                IssueTime = new IssueTimeType { Value = fatura.tarih },
                InvoiceTypeCode = new InvoiceTypeCodeType { Value = "SATIS" },
                Note = new[] {
                    new NoteType { Value = fatura.toplamtutar.YaziIleTutar() },
                    new NoteType { Value = "İş bu fatura muhteviyatına 7 gün içerisinde itiraz edilmediği taktirde aynen kabul edilmiş sayılır." },
                    new NoteType { Value = fatura.alicicarikod + " " + fatura.alicicariadi }
                },
                DocumentCurrencyCode = new DocumentCurrencyCodeType { Value = fatura.parakod },
                LineCountNumeric = new LineCountNumericType { Value = faturaici.Count },
                AdditionalDocumentReference = new[] {
                    new DocumentReferenceType {
                        ID = new IDType { Value = Guid.NewGuid().ToString() },
                        IssueDate = new IssueDateType { Value = fatura.tarih },
                        DocumentType = new DocumentTypeType { Value = "XSLT" },
                        Attachment = new AttachmentType {
                            EmbeddedDocumentBinaryObject = new EmbeddedDocumentBinaryObjectType {
                                characterSetCode = "UTF-8",
                                encodingCode = "Base64",
                                filename = "EArchiveInvoice.xslt",
                                mimeCode = "application/xml",
                                Value = Encoding.UTF8.GetBytes(
                                    new StreamReader(
                                        new FileStream(                                          
                                           attachment,
                                           FileMode.Open, FileAccess.Read
                                        ), Encoding.UTF8
                                    ).ReadToEnd())
                            }
                        }
                    },
                    new DocumentReferenceType {
                        ID = new IDType { Value = Guid.NewGuid().ToString() },
                        IssueDate = new IssueDateType { Value = fatura.tarih },
                        DocumentTypeCode = new DocumentTypeCodeType { Value = "SendingType" },
                        DocumentType = new DocumentTypeType { Value = "ELEKTRONIK" }
                    },
                },
                Signature = new[] {
                    new SignatureType {
                        ID = new IDType { schemeID = "VKN_TCKN", Value = "1234567890" },
                        SignatoryParty = new PartyType {
                            PartyIdentification = new[] {
                                new PartyIdentificationType { ID = new IDType { schemeID = "VKN", Value = "1234567890" } } },
                            PostalAddress = new AddressType {
                                Room = new RoomType { Value = "25" },
                                BlockName = new BlockNameType { Value = "A Blok" },
                                BuildingName = new BuildingNameType { Value = "Karanfil" },
                                BuildingNumber = new BuildingNumberType { Value = "345" },
                                CitySubdivisionName = new CitySubdivisionNameType { Value = "Çankaya" },
                                CityName = new CityNameType { Value = "Ankara" },
                                PostalZone = new PostalZoneType { Value = "06200" },
                                Country = new CountryType { Name = new NameType1 { Value = "Türkiye" } }
                            }
                        },
                        DigitalSignatureAttachment = new AttachmentType {
                            ExternalReference = new ExternalReferenceType {
                                URI = new URIType { Value = "#Signature_" + fatura.faturanu }
                            }
                        }
                    },
                },
                AccountingSupplierParty = new SupplierPartyType {
                    Party = new PartyType {
                        PartyIdentification = new[] {
                            new PartyIdentificationType { ID = new IDType { schemeID = "VKN", Value = "1234567890" } },
                            new PartyIdentificationType { ID = new IDType { schemeID = "MERSISNO", Value = "1234567890123456" } },
                        },
                        PartyName = new PartyNameType { Name = new NameType1 { Value = fatura.saticicariadi } },
                        PostalAddress = new AddressType {
                            Room = new RoomType { Value = "25" },
                            BlockName = new BlockNameType { Value = "A Blok" },
                            BuildingName = new BuildingNameType { Value = "Karanfil" },
                            BuildingNumber = new BuildingNumberType { Value = "345" },
                            CitySubdivisionName = new CitySubdivisionNameType { Value = "Çankaya" },
                            CityName = new CityNameType { Value = "Ankara" },
                            PostalZone = new PostalZoneType { Value = "06200" },
                            Country = new CountryType { Name = new NameType1 { Value = "Türkiye" } }
                        },
                        WebsiteURI = new WebsiteURIType { Value = "www.abcyazlim.com" },
                        Contact = new ContactType { ElectronicMail = new ElectronicMailType { Value = "info@abcyazilim.com" }, Telephone = new TelephoneType { Value = "555-5555555" } },
                        PartyTaxScheme = new PartyTaxSchemeType { TaxScheme = new TaxSchemeType { Name = new NameType1 { Value = "Çankaya VD" } } }
                    },
                },
                AccountingCustomerParty = new CustomerPartyType {
                    Party = new PartyType {
                        PartyIdentification = new[] {
                            new PartyIdentificationType { ID = new IDType { schemeID = "TCKN", Value = "12345678901" } },
                        },
                        PartyName = new PartyNameType { Name = new NameType1 { Value = "Hasan Yılmaz" } },
                        PostalAddress = new AddressType {
                            Room = new RoomType { Value = "14" },
                            BlockName = new BlockNameType { Value = "E Blok" },
                            BuildingName = new BuildingNameType { Value = "Aslan" },
                            BuildingNumber = new BuildingNumberType { Value = "1255" },
                            CitySubdivisionName = new CitySubdivisionNameType { Value = "Yenimahalle" },
                            CityName = new CityNameType { Value = "Ankara" },
                            PostalZone = new PostalZoneType { Value = "06400" },
                            Country = new CountryType { Name = new NameType1 { Value = "Türkiye" } }
                        },
                        Contact = new ContactType { ElectronicMail = new ElectronicMailType { Value = "hasanyilmaz@gmail.com" }, Telephone = new TelephoneType { Value = "444-4444444" } },
                        Person = new PersonType { FirstName = new FirstNameType { Value = "Hasan" }, FamilyName = new FamilyNameType { Value = "Yılmaz" } },
                    },
                },
                TaxTotal = new[] {
                    new TaxTotalType { TaxAmount = new TaxAmountType { Value = fatura.kdv },
                        TaxSubtotal = new[] {
                            new TaxSubtotalType {
                                TaxableAmount = new TaxableAmountType { currencyID = fatura.parakod, Value = fatura.nettutar },
                                TaxAmount = new TaxAmountType { currencyID = fatura.parakod, Value = fatura.kdv },
                                CalculationSequenceNumeric = new CalculationSequenceNumericType { Value = 1 },
                                TransactionCurrencyTaxAmount = new TransactionCurrencyTaxAmountType { currencyID = fatura.parakod, Value = fatura.kdv },
                                TaxCategory = new TaxCategoryType {
                                    TaxScheme = new TaxSchemeType {
                                        Name = new NameType1 { Value = "KDV" },
                                        TaxTypeCode = new TaxTypeCodeType { Value = "0015" }
                                    }
                                }
                            },
                        } }
                },
                LegalMonetaryTotal = new MonetaryTotalType {
                    LineExtensionAmount = new LineExtensionAmountType { Value = fatura.tutar },
                    TaxExclusiveAmount = new TaxExclusiveAmountType { Value = fatura.nettutar },
                    TaxInclusiveAmount = new TaxInclusiveAmountType { Value = fatura.toplamtutar },
                    AllowanceTotalAmount = new AllowanceTotalAmountType { Value = fatura.iskonto },
                    PayableAmount = new PayableAmountType { Value = fatura.toplamtutar }
                },
                InvoiceLine = getFaturaici(fatura.parakod, faturaici)
            };
            return invoice;
        }


        private InvoiceLineType[] getFaturaici(String parakod, vkFatura.faturaiciDataTable faturaici){
            var lines = new List<InvoiceLineType>();
            var source = faturaici.Rows;
            var lineNumber = 1;
            foreach(vkFatura.faturaiciRow x in faturaici.Rows){
                var line = new InvoiceLineType{
                    ID = new IDType { Value = lineNumber.ToString() },
                    Note = new[] { new NoteType { Value = x.urunkod + " - " + x.urunadi } },
                    InvoicedQuantity = new InvoicedQuantityType { unitCode = x.birimkod, Value = x.miktar },
                    LineExtensionAmount = new LineExtensionAmountType { currencyID = parakod, Value = x.nettutar },
                    AllowanceCharge = new[]{
                        new AllowanceChargeType{
                            ChargeIndicator = new ChargeIndicatorType{ Value = false },
                            MultiplierFactorNumeric = new MultiplierFactorNumericType{ Value = x.iskontooran  },
                            Amount = new AmountType2{ currencyID = parakod, Value = x.iskonto },
                            BaseAmount = new BaseAmountType{ currencyID = parakod, Value = x.tutar },
                        }
                    },
                    TaxTotal = new TaxTotalType{
                        TaxAmount = new TaxAmountType { currencyID = parakod, Value = x.kdv },
                        TaxSubtotal = new[]{
                            new TaxSubtotalType{
                                TaxableAmount = new TaxableAmountType{ currencyID = parakod, Value = x.nettutar },
                                TaxAmount = new TaxAmountType{ currencyID = parakod, Value = x.kdv },
                                CalculationSequenceNumeric = new CalculationSequenceNumericType{ Value = 1 },
                                Percent = new PercentType1{ Value = x.kdvoran * 100},
                                TaxCategory = new TaxCategoryType{
                                    TaxScheme = new TaxSchemeType{
                                        TaxTypeCode = new TaxTypeCodeType{ Value = "0015", name = "KDV" },
                                        Name = new NameType1{ Value = "KDV" }
                                    },
                                }
                            }
                        }
                    },
                    Item = new ItemType { Name = new NameType1 { Value = x.urunkod + " - " + x.urunadi } },
                    Price = new PriceType { PriceAmount = new PriceAmountType { currencyID = parakod, Value = x.fiyat } }
                };

                lineNumber++;
                lines.Add(line);
            };
            return lines.ToArray();
        }

       public  vkFatura FromInvoice(InvoiceType invoice)
        {
            throw new NotImplementedException();
        }
    }
}
