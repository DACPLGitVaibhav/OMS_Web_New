using System.ComponentModel.DataAnnotations;

namespace OMS_Template.ViewModels.AutoManual
{
    public class AutoManualViewModel
    {
        [Required(ErrorMessage = "Enter PPseqno")]
        public int PPSeqNo { get; set; }
        public bool IsAutoMode { get; set; }
    }
}
