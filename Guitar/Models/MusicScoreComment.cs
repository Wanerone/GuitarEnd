//------------------------------------------------------------------------------
// <auto-generated>
//     此代码已从模板生成。
//
//     手动更改此文件可能导致应用程序出现意外的行为。
//     如果重新生成代码，将覆盖对此文件的手动更改。
// </auto-generated>
//------------------------------------------------------------------------------

namespace Guitar.Models
{
    using System;
    using System.Collections.Generic;
    
    public partial class MusicScoreComment
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public MusicScoreComment()
        {
            this.MusicScoreReply = new HashSet<MusicScoreReply>();
        }
    
        public int Ms_commentid { get; set; }
        public int Ms_id { get; set; }
        public string content { get; set; }
        public System.DateTime Addtime { get; set; }
        public int User_id { get; set; }
    
        public virtual MusicScore MusicScore { get; set; }
        public virtual Users Users { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<MusicScoreReply> MusicScoreReply { get; set; }
    }
}
