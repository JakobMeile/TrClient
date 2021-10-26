// <copyright file="TrPageLevelItem.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

/// <summary>
/// Beskrivelse ... / Hjælpeklasse for ...
/// </summary>

//
// Class KlasseNavn
//
// Arver:       ingen
// Base for:    ingen
//
// Versionshistorik m.v. - testet? fungerer? dato?
namespace TrClient.Core
{
    using TrClient.Libraries;

    public abstract class TrPageLevelItem : TrItem
    {
        // ------------------------------------------------------------------------------------------------------------------------
        // enums                                                                                                              enums

        // ------------------------------------------------------------------------------------------------------------------------
        // abstract properties                                                                                  abstract properties

        // ------------------------------------------------------------------------------------------------------------------------
        // public properties                                                                                      public properties
        public string CoordsString { get; set; }

        public bool MarkToDeletion { get; set; }

        private bool hasCoords;

        public bool HasCoords
        {
            get
            {
                hasCoords = CoordsString != string.Empty;
                return hasCoords;
            }
        }

        private int leftBorder;

        public int LeftBorder
        {
            get
            {
                leftBorder = TrLibrary.GetLeftMostXcoord(CoordsString);
                return leftBorder;
            }
        }

        private int rightBorder;

        public int RightBorder
        {
            get
            {
                rightBorder = TrLibrary.GetRightMostXcoord(CoordsString);
                return rightBorder;
            }
        }

        private int topBorder;

        public int TopBorder
        {
            get
            {
                topBorder = TrLibrary.GetTopYcoord(CoordsString);
                return topBorder;
            }
        }

        private int bottomBorder;

        public int BottomBorder
        {
            get
            {
                bottomBorder = TrLibrary.GetBottomYcoord(CoordsString);
                return bottomBorder;
            }
        }

        private int hPos;

        public int Hpos
        {
            get
            {
                hPos = LeftBorder;
                return hPos;
            }
        }

        private int vPos;

        public int Vpos
        {
            get
            {
                vPos = TrLibrary.GetAverageYcoord(CoordsString);
                return vPos;
            }
        }

        // width??
        private int horizontalOrder;

        public int HorizontalOrder
        {
            get
            {
                horizontalOrder = (Hpos * 10_000) + Vpos;
                return horizontalOrder;
            }
        }

        private int verticalOrder;

        public int VerticalOrder
        {
            get
            {
                verticalOrder = (Vpos * 10_000) + Hpos;
                return verticalOrder;
            }
        }

        // ------------------------------------------------------------------------------------------------------------------------
        // protected properties                                                                                protected properties

        // ------------------------------------------------------------------------------------------------------------------------
        // private properties                                                                                    private properties

        // ------------------------------------------------------------------------------------------------------------------------
        // events                                                                                                            events

        // ------------------------------------------------------------------------------------------------------------------------
        // constructors                                                                                                constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="TrPageLevelItem"/> class.
        /// Default constructor.
        /// </summary>
        public TrPageLevelItem()
        {
            MarkToDeletion = false;
        }

        /// <summary>
        /// Non-default constructor
        /// </summary>

        // ------------------------------------------------------------------------------------------------------------------------
        // interface-implementing methods                                                            interface-implementing methods

        // ------------------------------------------------------------------------------------------------------------------------
        // abstract methods                                                                                        abstract methods

        // ------------------------------------------------------------------------------------------------------------------------
        // public override methods                                                                          public override methods

        /// <summary>
        /// Override af ToString()
        /// </summary>
        /// <returns>
        /// Ingenting (ikke implementeret)
        /// </returns>
        //public override string ToString()
        //{
        //    return "";
        //}

        // ------------------------------------------------------------------------------------------------------------------------
        // public methods                                                                                            public methods

        /// <summary>
        /// Override af ToString()
        /// </summary>
        /// <returns>
        /// Ingenting (ikke implementeret)
        /// </returns>
        //public override string ToString()
        //{
        //    return "";
        //}

        // ------------------------------------------------------------------------------------------------------------------------
        // private methods                                                                                          private methods
    }
}
