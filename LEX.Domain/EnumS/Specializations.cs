using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LEX.Domain.EnumS
{
    [Flags]
    public enum Specializations
    {
        None = 0,

        CriminalLaw = 1,
        CivilLaw = 2,
        CommercialLaw = 3,
        CorporateLaw = 4,
        FamilyLaw = 5,
        LaborLaw = 6,
        AdministrativeLaw = 7,
        TaxLaw = 8,
        RealEstateLaw = 9,
        IntellectualProperty = 10,
        InternationalLaw = 11,
        BankingAndFinance = 12,
        Arbitration = 13,
        //CriminalLaw = 1 << 0,       
        //CivilLaw = 1 << 1,          
        //CommercialLaw = 1 << 2,    
        //CorporateLaw = 1 << 3,     
        //FamilyLaw = 1 << 4,          
        //LaborLaw = 1 << 5,          
        //AdministrativeLaw = 1 << 6, 
        //TaxLaw = 1 << 7,           
        //RealEstateLaw = 1 << 8,      
        //IntellectualProperty = 1 << 9, 
        //InternationalLaw = 1 << 10, 
        //BankingAndFinance = 1 << 11, 
        //Arbitration = 1 << 12        
    }
}