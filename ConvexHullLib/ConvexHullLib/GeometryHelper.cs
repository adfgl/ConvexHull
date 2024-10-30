using System.Runtime.CompilerServices;

namespace ConvexHullLib
{
    public static class GeometryHelper
    {
        /// <summary>
        /// Calculates the signed distance from a point to the face's plane.
        /// </summary>
        /// <param name="nx">The X component of plane normal.</param>
        /// <param name="ny">The Y component of plane normal.</param>
        /// <param name="nz">The Z component of plane normal.</param>
        /// <param name="d">Distance from plane to origin.</param>
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
        public static double SignedDistanceToPlane(
            double nx, double ny, double nz, 
            double d, 
            double x, double y, double z)
        {
            return nx * x + ny * y + nz * z - d;
        }

        /// <summary>
        /// Calculates the plane equation from three points.
        /// </summary>
        /// <param name="ax">The X coordinate of the point A.</param>
        /// <param name="ay">The Y coordinate of the point A.</param>
        /// <param name="az">The Z coordinate of the point A.</param>
        /// <param name="bx">The X coordinate of the point B.</param>
        /// <param name="by">The Y coordinate of the point B.</param>
        /// <param name="bz">The Z coordinate of the point B.</param>
        /// <param name="cx">The X coordinate of the point C.</param>
        /// <param name="cy">The Y coordinate of the point C.</param>
        /// <param name="cz">The Z coordinate of the point C.</param>
        /// <param name="nx">The X component of plane normal.</param>
        /// <param name="ny">The Y component of plane normal.</param>
        /// <param name="nz">The Z component of plane normal.</param>
        /// <param name="d">Distance from plane to origin.</param>
        public static void CalculatePlane(
            double ax, double ay, double az, 
            double bx, double by, double bz, 
            double cx, double cy, double cz, 
            out double nx, out double ny, out double nz, 
            out double d)
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
            d = nx * ax + ny * ay + nz * az;
        }

        public static double SquareDistanceFromPointToLine(
            double ax, double ay, double az,
            double bx, double by, double bz,
            double x, double y, double z)
        {
            // Direction vector of the line (from A to B)
            double ABx = bx - ax;
            double ABy = by - ay;
            double ABz = bz - az;

            // Vector from A to P
            double APx = x - ax;
            double APy = y - ay;
            double APz = z - az;

            // Calculate the dot products
            double AB_AB = ABx * ABx + ABy * ABy + ABz * ABz; // ||AB||^2
            double AP_AB = APx * ABx + APy * ABy + APz * ABz; // AP · AB

            // Projection of AP onto AB
            double projFactor = AP_AB / AB_AB;
            double projX = projFactor * ABx;
            double projY = projFactor * ABy;
            double projZ = projFactor * ABz;

            // Distance vector from P to the projection of P onto the line
            double Dx = APx - projX;
            double Dy = APy - projY;
            double Dz = APz - projZ;

            // Calculate the distance
            return Dx * Dx + Dy * Dy + Dz * Dz;
        }

    }
}
