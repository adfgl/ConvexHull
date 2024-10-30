using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace ConvexHullLib
{
    public readonly struct Plane
    {
        public readonly double nx, ny, nz, distanceToOrigin;

        public Plane(double nx, double ny, double nz, double distanceToOrigin)
        {
            this.nx = nx;
            this.ny = ny;
            this.nz = nz;
            this.distanceToOrigin = distanceToOrigin;
        }

        public Plane(double ax, double ay, double az, double bx, double by, double bz, double cx, double cy, double cz)
        {
            // subtract vectors
            double xBA = bx - ax;
            double yBA = by - ay;
            double zBA = bz - az;

            double xCA = cx - ax;
            double yCA = cy - ay;
            double zCA = cz - az;

            // cross product
            double cpx = yBA * zCA - yCA * zBA;
            double cpy = zBA * xCA - zCA * xBA;
            double cpz = xBA * yCA - xCA * yBA;

            // normalize
            double len = Math.Sqrt(cpx * cpx + cpy * cpy + cpz * cpz);
            nx = cpx / len;
            ny = cpy / len;
            nz = cpz / len;

            // calculate distance to origin
            distanceToOrigin = nx * ax + ny * ay + nz * az;
        }

        /// <summary>
        /// Calculates the signed distance from a point to the face's plane.
        /// </summary>
        /// <param name="x">The X coordinate of the point.</param>
        /// <param name="y">The Y coordinate of the point.</param>
        /// <param name="z">The Z coordinate of the point.</param>
        /// <returns>
        /// The signed distance from the point to the face's plane. 
        /// A <b><c>positive</c></b> value indicates the point is in front of the plane, 
        /// a <b><c>negative</c></b> value indicates it is behind the plane, 
        /// and <b><c>zero</c></b> indicates it is directly on the plane.
        /// </returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public double SignedDistance(double x, double y, double z)
        {
            return nx * x + ny * y + nz * z - distanceToOrigin;
        }
    }
}
