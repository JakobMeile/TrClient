// <copyright file="TrPageLevelItem.cs" company="Kyrillos">
// Copyright (c) Jakob K. Meile 2021.
// </copyright>

/// <summary>
/// Contains public abstract class TrPageLevelItem.
/// </summary>

namespace TranskribusClient2.Core
{
    using System;
    using System.Linq;
    
    /// <summary>
    /// Base class for all items below page level, which have a position and hence coordinates,
    /// i.e. TrRegion (TrTextRegion and TrTableRegion), TrTextLine, TrCell.
    /// Inherits <see cref="TrItem"/>.
    /// </summary>
    public abstract class TrPageLevelItem : TrItem
    {
        // ------------------------------------------------------------------------------------------------------------------------
        // 1. Constants 

        // ------------------------------------------------------------------------------------------------------------------------
        // 2. Fields 

        /// <summary>
        /// Holds the page number of the item.
        /// </summary>
        private protected int _pageNumber;

        /// <summary>
        /// Holds the number of the item.
        /// </summary>
        private protected int _number;

        /// <summary>
        /// Holds the string with coordinates.
        /// </summary>
        private protected string _coordinatesString;

        ///// <summary>
        ///// Holds a boolean flag indicating whether the item can be deleted.
        ///// </summary>
        //private protected bool _markToDeletion;

        /// <summary>
        /// Holds a boolean flag indicating whether the item has coordinates.
        /// </summary>
        private protected bool _hasCoordinates;

        /// <summary>
        /// Holds a value indicating the horizontal position of the item.
        /// </summary>
        private protected int _horizontalPosition;

        /// <summary>
        /// Holds a value indicating the horizontal end position of the item.
        /// </summary>
        private protected int _horizontalEndPosition;

        /// <summary>
        /// Holds a value indicating the vertical position of the item.
        /// </summary>
        private protected int _verticalPosition;
        
        /// <summary>
        /// Holds a value indicating the width of the item.
        /// </summary>
        private protected int _width;

        /// <summary>
        /// Holds a value indicating the height of the item.
        /// </summary>
        private protected int _height;

        /// <summary>
        /// Holds a value indicating the left border of the item.
        /// </summary>
        private protected int _leftBorder;

        /// <summary>
        /// Holds a value indicating the right border of the item.
        /// </summary>
        private protected int _rightBorder;

        /// <summary>
        /// Holds a value indicating the top border of the item.
        /// </summary>
        private protected int _topBorder;

        /// <summary>
        /// Holds a value indicating the bottom border of the item.
        /// </summary>
        private protected int _bottomBorder;

        /// <summary>
        /// Holds a value indicating the horizontal order of the item.
        /// </summary>
        private protected int _horizontalOrder;

        /// <summary>
        /// Holds a value indicating the vertial order of the item.
        /// </summary>
        private protected int _verticalOrder;

        // ------------------------------------------------------------------------------------------------------------------------
        // 3. Constructors 

        /// <summary>
        /// Initializes a new instance of the <see cref="TrPageLevelItem"/> class.
        /// Default constructor.
        /// </summary>
        public TrPageLevelItem() 
        {
        }

        // ------------------------------------------------------------------------------------------------------------------------
        // 4. Finalizers 

        // ------------------------------------------------------------------------------------------------------------------------
        // 5. Delegates 

        // ------------------------------------------------------------------------------------------------------------------------
        // 6. Events 

        // ------------------------------------------------------------------------------------------------------------------------
        // 7. Enums 

        // ------------------------------------------------------------------------------------------------------------------------
        // 8. Interface implementations 

        // ------------------------------------------------------------------------------------------------------------------------
        // 9. Properties 

        /// <summary>
        /// Gets the page number of the item - via parents.
        /// </summary>
        public abstract int PageNumber { get; }

        /// <summary>
        /// Gets or sets the number of the item.
        /// </summary>
        public abstract int Number { get; set; }

        /// <summary>
        /// Gets or sets the string with coordinates.
        /// </summary>
        /// <exception cref="ArgumentNullException">Throws exception if set to null.</exception>
        /// <exception cref="ArgumentException">Throws exception if set to empty string.</exception>
        public string CoordinatesString 
        { 
            get
            {
                return _coordinatesString;
            }

            set
            {
                _coordinatesString = value;
                if (_coordinatesString == null)
                {
                    throw new ArgumentNullException("A coordinate string can't be null.");
                }
                else if (_coordinatesString == string.Empty)
                {
                    throw new ArgumentException("A coordinate string can't be empty.");
                }
            }
        }

        ///// <summary>
        ///// Gets or sets a value indicating whether the item can be deleted.
        ///// </summary>
        //public bool MarkToDeletion
        //{
        //    get
        //    {
        //        return _markToDeletion;
        //    }

        //    set
        //    {
        //        _markToDeletion = value;
        //    }
        //}

        /// <summary>
        /// Gets a value indicating whether the item has got coordinates.
        /// </summary>
        public bool HasCoordinates
        {
            get
            {
                _hasCoordinates = CoordinatesString != string.Empty;
                return _hasCoordinates;
            }
        }

        /// <summary>
        /// Gets the horizontal starting position of the item.
        /// </summary>
        public int HorizontalPosition
        {
            get
            {
                _horizontalPosition = GetLeftMostX(CoordinatesString);
                return _horizontalPosition;
            }
        }

        /// <summary>
        /// Gets the horizontal ending position of the item.
        /// </summary>
        public int HorizontalEndPosition
        {
            get
            {
                _horizontalEndPosition = GetRightMostX(CoordinatesString);
                return _horizontalEndPosition;
            }
        }

        /// <summary>
        /// Gets the <b>average</b> vertical position of the item.
        /// </summary>
        /// <remarks>
        /// Remember that vertical zero is on top of page!
        /// </remarks>
        public int VerticalPosition
        {
            get
            {
                _verticalPosition = GetAverageY(CoordinatesString);
                return _verticalPosition;
            }
        }

        /// <summary>
        /// Gets the width of the item.
        /// </summary>
        public int Width
        {
            get
            {
                _width = HorizontalEndPosition - HorizontalPosition;
                return _width;
            }
        }

        /// <summary>
        /// Gets the height of the item.
        /// </summary>
        public int Height
        {
            get
            {
                _height = BottomBorder - TopBorder;
                return _height;
            }
        }

        /// <summary>
        /// Gets the left border of the item.
        /// </summary>
        public int LeftBorder
        {
            get
            {
                _leftBorder = HorizontalPosition;
                return _leftBorder;
            }
        }

        /// <summary>
        /// Gets the right border of the item.
        /// </summary>
        public int RightBorder
        {
            get
            {
                _rightBorder = HorizontalEndPosition;
                return _rightBorder;
            }
        }

        /// <summary>
        /// Gets the top border of the item.
        /// </summary>
        /// <remarks>
        /// Remember that vertical zero is on top of page!
        /// </remarks>
        public int TopBorder
        {
            get
            {
                _topBorder = GetTopMostY(CoordinatesString);
                return _topBorder;
            }
        }

        /// <summary>
        /// Gets the bottom border of the item.
        /// </summary>
        /// <remarks>
        /// Remember that vertical zero is on top of page!
        /// </remarks>
        public int BottomBorder
        {
            get
            {
                _bottomBorder = GetBottomMostY(CoordinatesString);
                return _bottomBorder;
            }
        }

        /// <summary>
        /// Gets an arbitrary and calculated number, that expresses the horizontal order of the item.
        /// </summary>
        public int HorizontalOrder
        {
            get
            {
                _horizontalOrder = (HorizontalPosition * 10_000) + VerticalPosition;
                return _horizontalOrder;
            }
        }

        /// <summary>
        /// Gets an arbitrary and calculated number, that expresses the vertical order of the item.
        /// </summary>
        public int VerticalOrder
        {
            get
            {
                _verticalOrder = (VerticalPosition * 10_000) + HorizontalPosition;
                return _verticalOrder;
            }
        }

        private static int GetLeftMostX(string coordinates)
        {
            int tempResult = -1;
            if (coordinates != string.Empty)
            {
                string temp = coordinates.Replace(" ", ";");
                var pointsArray = temp.Split(';').ToArray();
                int pointsCount = pointsArray.Length;

                int minX = 10000;

                for (int i = 0; i < pointsCount; i++)
                {
                    int commaPos = pointsArray[i].IndexOf(",");
                    int x = (int)Convert.ToInt32(pointsArray[i].Substring(0, commaPos));
                    if (x < minX)
                    {
                        minX = x;
                    }
                }

                tempResult = minX;
            }

            return tempResult;
        }

        private static int GetRightMostX(string coordinates)
        {
            int tempResult = -1;
            if (coordinates != string.Empty)
            {
                string temp = coordinates.Replace(" ", ";");
                var pointsArray = temp.Split(';').ToArray();
                int pointsCount = pointsArray.Length;

                int maxX = 0;

                for (int i = 0; i < pointsCount; i++)
                {
                    int commaPos = pointsArray[i].IndexOf(",");
                    if (commaPos > -1)
                    {
                        int x = (int)Convert.ToInt32(pointsArray[i].Substring(0, commaPos));
                        if (x > maxX)
                        {
                            maxX = x;
                        }
                    }
                }

                tempResult = maxX;
            }

            return tempResult;
        }

        private static int GetTopMostY(string coordinates)
        {
            int tempResult = -1;
            if (coordinates != string.Empty)
            {
                string temp = coordinates.Replace(" ", ";");
                var pointsArray = temp.Split(';').ToArray();
                int pointsCount = pointsArray.Length;

                int topY = 20000;

                for (int i = 0; i < pointsCount; i++)
                {
                    int commaPos = pointsArray[i].IndexOf(",");
                    int y = (int)Convert.ToInt32(pointsArray[i].Substring(commaPos + 1));
                    if (y < topY)
                    {
                        topY = y;
                    }
                }

                tempResult = topY;
            }

            return tempResult;
        }

        private static int GetBottomMostY(string coordinates)
        {
            int tempResult = -1;
            if (coordinates != string.Empty)
            {
                string temp = coordinates.Replace(" ", ";");
                var pointsArray = temp.Split(';').ToArray();
                int pointsCount = pointsArray.Length;

                int bottomY = 0;

                for (int i = 0; i < pointsCount; i++)
                {
                    int commaPos = pointsArray[i].IndexOf(",");
                    int y = (int)Convert.ToInt32(pointsArray[i].Substring(commaPos + 1));
                    if (y > bottomY)
                    {
                        bottomY = y;
                    }
                }

                tempResult = bottomY;
            }

            return tempResult;
        }

        private static int GetAverageY(string coordinates)
        {
            int tempResult = -1;
            if (coordinates != string.Empty)
            {
                string temp = coordinates.Replace(" ", ";");
                var pointsArray = temp.Split(';').ToArray();
                int pointsCount = pointsArray.Length;

                int sumYcoord = 0;

                for (int i = 0; i < pointsCount; i++)
                {
                    int commaPos = pointsArray[i].IndexOf(",");
                    int y = (int)Convert.ToInt32(pointsArray[i].Substring(commaPos + 1));

                    sumYcoord += y;
                }

                tempResult = (int)(sumYcoord / pointsCount);
            }

            return tempResult;
        }



        // ------------------------------------------------------------------------------------------------------------------------
        // 12. Structs 

        // ------------------------------------------------------------------------------------------------------------------------
        // 13. Classes 
    }
}
