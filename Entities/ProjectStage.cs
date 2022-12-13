using System;

namespace HeiLiving.Quotes.Api.Entities
{
    public class ProjectStage
    {        
        public Guid ProjectId { get; set; }
        public Guid StageId { get; set; }
        
        //-----------------------------
        //Relationships
        public Project Project { get; set; }
        public Stage Stage { get; set; }
    }
}