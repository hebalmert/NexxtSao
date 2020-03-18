using Resources;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace NexxtSao.Models.MVC
{
    public class ClientHistory
    {
        [Key]
        public int ClientHistoryId { get; set; }

        [Required(ErrorMessageResourceType = typeof(Resource), ErrorMessageResourceName = "Msg_Required")]
        [Range(1, double.MaxValue, ErrorMessageResourceType = typeof(Resource), ErrorMessageResourceName = "Msg_Range")]
        [Index("ClientHistory_ClientId_Company_Index", 1, IsUnique = true)]
        [Display(ResourceType = typeof(Resource), Name = "ClientHisotry_Model_Company")]
        public int CompanyId { get; set; }

        [Required(ErrorMessageResourceType = typeof(Resource), ErrorMessageResourceName = "Msg_Required")]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        [Display(ResourceType = typeof(Resource), Name = "ClientHistory_Model_Date")]
        public DateTime Date { get; set; }

        [Required(ErrorMessageResourceType = typeof(Resource), ErrorMessageResourceName = "Msg_Required")]
        [Range(1, double.MaxValue, ErrorMessageResourceType = typeof(Resource), ErrorMessageResourceName = "Msg_Range")]
        [Index("ClientHistory_ClientId_Company_Index", 2, IsUnique = true)]
        [Display(ResourceType = typeof(Resource), Name = "ClientHistory_Model_Cliente")]
        public int ClientId { get; set; }

        [Display(ResourceType = typeof(Resource), Name = "ClientHistory_Model_MedicoActual")]
        public bool MedicoActual { get; set; }

        [Display(ResourceType = typeof(Resource), Name = "ClientHistory_Model_Medicamento")]
        public bool TomaMedicamento { get; set; }

        [Display(ResourceType = typeof(Resource), Name = "ClientHistory_Model_Alergias")]
        public bool Alergias { get; set; }

        [Display(ResourceType = typeof(Resource), Name = "ClientHistory_Model_Cardiopatias")]
        public bool Cardiopatias { get; set; }

        [Display(ResourceType = typeof(Resource), Name = "ClientHistory_Model_PresionArterial")]
        public bool PresionArterial { get; set; }

        [Display(ResourceType = typeof(Resource), Name = "ClienteHistory_Model_Embarazo")]
        public bool Embarazo { get; set; }

        [Display(ResourceType = typeof(Resource), Name = "ClientHistory_Model_Diabetes")]
        public bool Diabetes { get; set; }

        [Display(ResourceType = typeof(Resource), Name = "ClientHistory_Model_Hepatitis")]
        public bool Hepatitis { get; set; }

        [Display(ResourceType = typeof(Resource), Name = "ClientHistory_Model_Irradiaciones")]
        public bool Irradiaciones { get; set; }

        [Display(ResourceType = typeof(Resource), Name = "ClientHistory_Model_Discracias")]
        public bool DiscraciasSanguineas { get; set; }

        [Display(ResourceType = typeof(Resource), Name = "ClientHistory_Model_FiebreReumatica")]
        public bool FiebreReumatica { get; set; }

        [Display(ResourceType = typeof(Resource), Name = "ClientHistory_Model_Ranales")]
        public bool Renales { get; set; }

        [Display(ResourceType = typeof(Resource), Name = "ClientHistory_Model_Inmunosupresion")]
        public bool Inmunosupresion { get; set; }

        [Display(ResourceType = typeof(Resource), Name = "ClientHistory_Model_TransEmoacional")]
        public bool TranstornoEmaocional { get; set; }

        [Display(ResourceType = typeof(Resource), Name = "ClientHisotry_Model_TransRepiratorio")]
        public bool TranstornoRespiratorio { get; set; }

        [Display(ResourceType = typeof(Resource), Name = "ClientHisotry_Model_TransGastrico")]
        public bool TranstornoGastrico { get; set; }

        [Display(ResourceType = typeof(Resource), Name = "ClientHisotry_Model_Epilepsia")]
        public bool Epilepsia { get; set; }

        [Display(ResourceType = typeof(Resource), Name = "ClientHistory_Model_Cirugias")]
        public bool Cirugias { get; set; }

        [Display(ResourceType = typeof(Resource), Name = "ClientHistoy_Model_EnfermedadOral")]
        public bool EnfermedadOral { get; set; }

        [Display(ResourceType = typeof(Resource), Name = "ClientHistory_Model_Fumalico")]
        public bool FumaLicor { get; set; }

        [Display(ResourceType = typeof(Resource), Name = "ClientHistoy_Model_OtrasAlteraciones")]
        public bool OtrasAlteraciones { get; set; }

        [Required(ErrorMessageResourceType = typeof(Resource), ErrorMessageResourceName = "Msg_Required")]
        [MaxLength(256, ErrorMessageResourceType = typeof(Resource), ErrorMessageResourceName = "Msg_MaxLength")]
        [DataType(DataType.MultilineText)]
        [Display(ResourceType = typeof(Resource), Name = "ClienteHistory_Model_Detalle")]
        public string Detalle { get; set; }

        public virtual Company Company { get; set; }

        public virtual Client Client { get; set; }
    }
}