#region Modification Log
/*------------------------------------------------------------------------------------------------------------------------------------------------- 
   System      -   OrderWave
   Module      -   Core

Modification History:
==================================================================================================================================================
Date              Version      		Modify by              					Description
--------------------------------------------------------------------------------------------------------------------------------------------------
31/05/2015         	  1.0           Anuruddha   					        Initial Version
--------------------------------------------------------------------------------------------------------------------------------------------------*/
#endregion
#region Namespace
using System;
#endregion	  

namespace OrderWave.Core.Attributes {
    public class ColumnAttribute : Attribute {
        public string FieldName { get; set; } = string.Empty;
        public string DisplayName { get; set; } = string.Empty;
        public int SortOrder { get; set; } = 0;
        public bool IsUnique { get; set; } = false;
        public bool IsIdentity { get; set; } = false;
        public bool Visible { get; set; } = false;
        public bool IsNotBoolean { get; set; } = false;

        //public ColumnAttribute(string fieldName)
        //    : this(fieldName, string.Empty, 0, false, false, true) {
        //}

        public ColumnAttribute(string displayName,bool isFieldColumn=false) {
            if (isFieldColumn) {
                this.Init(displayName, displayName, 0, false, false, true);
            } else {
                this.Init(string.Empty, displayName, 0, false, false, true);
            }
        }

        public ColumnAttribute(string fieldName, string displayName)
            : this(fieldName, displayName, 0, false, false, true) {
        }

        public ColumnAttribute(string fieldName, string displayName, int sortOrder, bool Visible)
            : this(fieldName, displayName, 0, false, false, Visible) {
        }

        public ColumnAttribute(string fieldName, string displayName, int sortOrder)
            : this(fieldName, displayName, sortOrder, false, false, true) {
        }

        public ColumnAttribute(string fieldName, string displayName, bool Visible)
            : this(fieldName, displayName, 0, false, false, Visible) {
        }

        public ColumnAttribute(string fieldName, string displayName, int sortOrder, bool IsUnique, bool isIdentity, bool visible) {
            this.Init(fieldName, displayName, sortOrder, IsUnique, isIdentity, visible);
        }

        private void Init(string fieldName, string displayName, int sortOrder, bool IsUnique, bool isIdentity, bool visible) {
            this.FieldName = fieldName;
            this.DisplayName = displayName;
            this.SortOrder = sortOrder;
            this.IsUnique = IsUnique;
            this.IsIdentity = isIdentity;
            this.Visible = visible;
        }

        //public ColumnAttribute(string fieldName, bool isNotBoolean)
        //    : this(fieldName, string.Empty, 0, false, false, true) {
        //    this.IsNotBoolean = isNotBoolean;
        //}

    }
}
