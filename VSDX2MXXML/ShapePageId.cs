using System;
using System.Collections.Generic;
using System.Text;

namespace VSDX2MXXML
{
    class ShapePageId
    {
        private int pageNumber;

        private int Id;

        public ShapePageId(int pageNumber, int Id)
        {
            this.pageNumber = pageNumber;
            this.Id = Id;
        }

        public int getId()
        {
            return Id;
        }

        public int getPageNumber()
        {
            return pageNumber;
        }


        public override bool Equals(Object obj)
        {
            if (obj == null || GetType() != obj.GetType())
            {
                return false;
            }

            ShapePageId other = (ShapePageId)obj;

            if (this.pageNumber != other.pageNumber || this.Id != other.Id)
            {
                return false;
            }
            return true;
        }


        public override int GetHashCode()
        {
            return 100000 * this.pageNumber + this.Id;
        }
    }
}
