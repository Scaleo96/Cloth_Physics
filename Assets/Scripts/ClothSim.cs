using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Diagnostics;

public class ClothSim : MonoBehaviour
{
    public enum ClothType { spring, constraint }
    [SerializeField]
    private ClothType clothType = ClothType.spring;

    [SerializeField]
    private bool constraintOnlyCollision = true;

    [SerializeField]
    int gridSizeInit = 15;
    private int gridSize;

    [SerializeField]
    ClothParticle[,] particles;
    List<ClothSpring> springs = new List<ClothSpring>();
    public List<ClothConstraint> constraints = new List<ClothConstraint>();

    [SerializeField]
    float radius = 5f;
    [SerializeField]
    float tenstion;
    [SerializeField]
    int constraintUpdate = 10;

    [SerializeField]
    Vector3 wind = Vector3.zero;
    [SerializeField]
    Vector3 gravity = new Vector3(0, -1.82f, 0);
    [SerializeField]
    float damp = 0.2f;

    private ClothParticle selectedParticle;

    [SerializeField]
    MeshRenderer selectedParticleMesh;
    private int connections;
    [SerializeField]
    private Text tConnections;
    [SerializeField]
    private Text ticks;


    [SerializeField]
    private float particleMass = 0.01f;
    [SerializeField]
    private float particleCollider = 0.01f;

    [SerializeField]
    bool sheerGrid = false;
    [SerializeField]
    bool bendGrid = false;
    [SerializeField]
    bool checkSelfCollision = true;

    [SerializeField]
    float particleMoveSpeed = 0.01f;

    [SerializeField]
    public List<ClothCollider> colliders = new List<ClothCollider>();

    private Vector3 clothAvarage = Vector3.zero;

    bool currentCollision = false;

    Vector3 oldMousePos;

    public ClothType ClothTypes { get { return clothType; } set { clothType = value; } }
    public int GridSize { get { return gridSizeInit; } set { gridSizeInit = value; } }
    public float Radius { get { return radius; } set { radius = value; } }
    public float Tension { get { return tenstion; } set { tenstion = value; } }
    public int ConstraintUpdate { get { return constraintUpdate; } set { constraintUpdate = value; } }
    public float ParticleMass { get { return particleMass; } set { particleMass = value; } }
    public float Damp { get { return damp; } set { damp = value; } }
    public bool SheerGrid { get { return sheerGrid; } set { sheerGrid = value; } }
    public bool BendGrid { get { return bendGrid; } set { bendGrid = value; } }
    public bool ConstraintOnlyCollision { get { return constraintOnlyCollision; } set { constraintOnlyCollision = value; } }
    public Vector3 Gravity { get { return gravity; } set { gravity = value; } }
    public Vector3 Wind { get { return wind; } set { wind = value; } }
    public Vector3 ClothAvarage { get { return clothAvarage; } set { clothAvarage = value; } }

    private MeshFilter filter;
    private MeshRenderer meshRenderer;

    List<Vector3> vertices = new List<Vector3>();
    List<Vector3> normals = new List<Vector3>();
    List<Vector2> uvs = new List<Vector2>();

    [SerializeField]
    GameObject linePrefab;
    private List<LineRenderer> gridLines = new List<LineRenderer>();

    // Start is called before the first frame update
    void Start()
    {
        filter = GetComponent<MeshFilter>();
        meshRenderer = GetComponent<MeshRenderer>();

        Reset();
    }

    Mesh GenerateMesh()
    {
        Mesh mesh = new Mesh();

        vertices.Clear();
        normals.Clear();
        uvs.Clear();

        float x = 0;
        for (int i = 0; i < particles.GetLength(0); i++)
        {
            float z = 0;
            for (int j = 0; j < particles.GetLength(0); j++)
            {
                vertices.Add(new Vector3(x, 0, z));
                normals.Add(Vector3.forward);
                uvs.Add(new Vector2(i / (float)gridSize, j / (float)gridSize));

                z = z + radius;
            }
            x = x + radius;
        }


        int count = gridSize - 1;
        int[] triangles = new int[count * count * 12];
        for (int ti = 0, vi = 0, y = 0; y < count; y++, vi++)
        {
            for (int i = 0; i < count; i++, ti += 12, vi++)
            {
                triangles[ti] = vi;
                triangles[ti + 3] = triangles[ti + 2] = vi + 1;
                triangles[ti + 4] = triangles[ti + 1] = vi + count + 1;
                triangles[ti + 5] = vi + count + 2;

                triangles[ti + 6] = vi;
                triangles[ti + 10] = triangles[ti + 7] = vi + 1;
                triangles[ti + 9] = triangles[ti + 8] = vi + count + 1;
                triangles[ti + 11] = vi + count + 2;
            }
        }

        mesh.SetVertices(vertices);
        mesh.SetNormals(normals);
        mesh.SetUVs(0, uvs);
        mesh.triangles = triangles;

        return mesh;
    }

    public void Reset()
    {
        constraints.Clear();
        springs.Clear();
        clothAvarage = Vector3.zero;

        gridSize = gridSizeInit;

        particles = new ClothParticle[gridSize, gridSize];

        float x = 0;
        for (int i = 0; i < particles.GetLength(0); i++)
        {
            float z = 0;
            for (int j = 0; j < particles.GetLength(1); j++)
            {
                ClothParticle p = new ClothParticle(new Vector3(x, 0, z), particleMass, particleCollider);
                particles[i, j] = p;
                z = z + radius;
                clothAvarage += new Vector3(0, -(radius / (gridSize) / 2), radius / gridSize);
            }
            x = x + radius;
            clothAvarage += new Vector3(radius / 2, 0, 0);
        }

        for (int i = 0; i < particles.GetLength(0); i++)
        {
            for (int j = 0; j < particles.GetLength(1); j++)
            {

                ClothParticle mainParticle = particles[i, j];
                ClothParticle subParticle;

                if (i < particles.GetLength(0) - 1)
                {
                    subParticle = particles[i + 1, j];
                    ClothSpring s1 = new ClothSpring(mainParticle, subParticle, tenstion);
                    springs.Add(s1);
                    ClothConstraint c1 = new ClothConstraint(mainParticle, subParticle);
                    constraints.Add(c1);
                    mainParticle.constraints.Add(c1);
                    subParticle.constraints.Add(c1);
                }

                if (j < particles.GetLength(1) - 1)
                {
                    subParticle = particles[i, j + 1];
                    ClothSpring s2 = new ClothSpring(mainParticle, subParticle, tenstion);
                    springs.Add(s2);
                    ClothConstraint c2 = new ClothConstraint(mainParticle, subParticle);
                    constraints.Add(c2);
                    mainParticle.constraints.Add(c2);
                    subParticle.constraints.Add(c2);
                }


                if (sheerGrid)
                {
                    if (j < particles.GetLength(0) - 1 && i < particles.GetLength(1) - 1)
                    {
                        subParticle = particles[i + 1, j + 1];
                        ClothSpring s3 = new ClothSpring(mainParticle, subParticle, tenstion);
                        springs.Add(s3);
                        ClothConstraint c3 = new ClothConstraint(mainParticle, subParticle);
                        constraints.Add(c3);
                        mainParticle.constraints.Add(c3);
                        subParticle.constraints.Add(c3);
                    }

                    if (j > 0 && i < particles.GetLength(1) - 1)
                    {
                        subParticle = particles[i + 1, j - 1];
                        ClothSpring s4 = new ClothSpring(mainParticle, subParticle, tenstion);
                        springs.Add(s4);
                        ClothConstraint c4 = new ClothConstraint(mainParticle, subParticle);
                        constraints.Add(c4);
                        mainParticle.constraints.Add(c4);
                        subParticle.constraints.Add(c4);
                    }

                }

                if (bendGrid)
                {
                    if (i < particles.GetLength(0) - 2)
                    {
                        subParticle = particles[i + 2, j];
                        ClothSpring s5 = new ClothSpring(mainParticle, subParticle, tenstion);
                        springs.Add(s5);
                        ClothConstraint c5 = new ClothConstraint(mainParticle, subParticle);
                        constraints.Add(c5);
                        mainParticle.constraints.Add(c5);
                        subParticle.constraints.Add(c5);
                    }

                    if (j < particles.GetLength(1) - 2)
                    {
                        subParticle = particles[i, j + 2];
                        ClothSpring s6 = new ClothSpring(mainParticle, subParticle, tenstion);
                        springs.Add(s6);
                        ClothConstraint c6 = new ClothConstraint(mainParticle, subParticle);
                        constraints.Add(c6);
                        mainParticle.constraints.Add(c6);
                        subParticle.constraints.Add(c6);
                    }

                    if (j < particles.GetLength(0) - 2 && i < particles.GetLength(1) - 2)
                    {
                        subParticle = particles[i + 2, j + 2];
                        ClothSpring s7 = new ClothSpring(mainParticle, subParticle, tenstion);
                        springs.Add(s7);
                        ClothConstraint c7 = new ClothConstraint(mainParticle, subParticle);
                        constraints.Add(c7);
                        mainParticle.constraints.Add(c7);
                        subParticle.constraints.Add(c7);

                    }

                    if (j > 1 && i < particles.GetLength(1) - 2)
                    {
                        subParticle = particles[i + 2, j - 2];
                        ClothSpring s8 = new ClothSpring(mainParticle, subParticle, tenstion);
                        springs.Add(s8);
                        ClothConstraint c8 = new ClothConstraint(mainParticle, subParticle);
                        constraints.Add(c8);
                        mainParticle.constraints.Add(c8);
                        subParticle.constraints.Add(c8);
                    }
                }
            }
        }


        particles[0, gridSize - 1].posLocked = true;
        particles[0, gridSize - 1].startPos = particles[0, gridSize - 1].pos;
        particles[gridSize - 1, gridSize - 1].posLocked = true;
        particles[gridSize - 1, gridSize - 1].startPos = particles[gridSize - 1, gridSize - 1].pos;


        selectedParticle = particles[gridSize - 1, gridSize - 1];

        filter.mesh = GenerateMesh();

        CreateWireframe();
    }

    private void CreateWireframe()
    {
        if (linePrefab.GetComponent<LineRenderer>() != null)
        {
            foreach (LineRenderer lr in gridLines)
            {
                Destroy(lr.gameObject);
            }
            gridLines.Clear();
            foreach (ClothConstraint c in constraints)
            {
                GameObject line = Instantiate(linePrefab, Vector3.zero, linePrefab.transform.rotation);
                gridLines.Add(line.GetComponent<LineRenderer>());
                line.SetActive(!meshRenderer.enabled);
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Vector3 mousePos = Input.mousePosition;

            float distance = (Camera.main.WorldToScreenPoint(particles[0, 0].pos) - mousePos).magnitude;

            ClothParticle temp = particles[0, 0];

            foreach (ClothParticle p in particles)
            {
                if ((Camera.main.WorldToScreenPoint(p.pos) - mousePos).magnitude < distance)
                {
                    distance = (Camera.main.WorldToScreenPoint(p.pos) - mousePos).magnitude;
                    temp = p;
                }
            }

            if (temp != null)
                selectedParticle = temp;

            tConnections.text = selectedParticle.constraints.Count.ToString();
        }

        if (Input.GetMouseButton(1))
        {
            selectedParticle.moving = true;
            selectedParticle.pos += Camera.main.transform.right * ((Input.mousePosition.x - oldMousePos.x) * particleMoveSpeed * (selectedParticle.pos - Camera.main.transform.position).magnitude);
            selectedParticle.pos += Camera.main.transform.up * ((Input.mousePosition.y - oldMousePos.y) * particleMoveSpeed * (selectedParticle.pos - Camera.main.transform.position).magnitude);
        }


        if (Input.GetMouseButtonUp(1))
        {
            selectedParticle.moving = false;
            if (clothType == ClothType.spring)
            {
                selectedParticle.acceleration = Vector3.zero;
            }
        }

        if (Input.GetKeyDown(KeyCode.P))
        {
            selectedParticle.posLocked = !selectedParticle.posLocked;
            if (clothType == ClothType.spring)
            {
                selectedParticle.acceleration = Vector3.zero;
            }
        }

        oldMousePos = Input.mousePosition;
    }

    private void FixedUpdate()
    {
        Stopwatch sp = new Stopwatch();
        sp.Start();

        //Update constrints if that is the type of the cloth is currently colliding with something
        if (clothType == ClothType.constraint || currentCollision && constraintOnlyCollision)
        {

            for (int i = 0; i < constraintUpdate; i++)
            {
                foreach (ClothConstraint c in constraints)
                {
                    c.SatisfyConstraint();
                }
            }
        }

        //Update springs if that is the type of the cloth is currently not colliding with something
        if (clothType == ClothType.spring && !currentCollision || clothType == ClothType.spring && !constraintOnlyCollision)
        {
            //Debug.Log("No collision");
            foreach (ClothConstraint c in constraints)
            {
                c.restLegth = (c.particle1.pos - c.particle2.pos).magnitude;
            }

            foreach (ClothSpring s in springs)
            {

                float tension = s.springTension;
                if (s.particle1.posLocked || s.particle2.posLocked)
                    tension *= 2;

                Vector3 dir = s.particle1.pos - s.particle2.pos;
                Vector3 springLen = dir.normalized * s.length;
                s.particle1.acceleration -= ((tension * (dir - springLen)) * (1 / s.particle1.mass)) * Time.fixedDeltaTime;
                s.particle2.acceleration += ((tension * (dir - springLen)) * (1 / s.particle2.mass)) * Time.fixedDeltaTime;
            }
        }

        bool collision = false;
        for (int i = 0; i < particles.GetLength(0); i++)
        {
            for (int j = 0; j < particles.GetLength(1); j++)
            {

                particles[i, j].acceleration += gravity * Time.fixedDeltaTime;

                if (i < gridSize - 1 && j < gridSize - 1)
                {
                    AddWind(particles[i + 1, j], particles[i, j], particles[i, j + 1], wind * ((Mathf.Sin(Time.time) + 3) / 4));
                    AddWind(particles[i + 1, j + 1], particles[i + 1, j], particles[i, j + 1], wind * ((Mathf.Sin(Time.time) + 3) / 4));
                }

                if (checkSelfCollision)
                {
                    for (int k = 0; k < particles.GetLength(0); k++)
                    {
                        for (int l = 0; l < particles.GetLength(0); l++)
                        {
                            if (particles[i, j] == particles[k, l])
                            {
                                continue;
                            }

                            Vector3 dir = particles[i, j].pos - particles[k, l].pos;

                            if (dir.magnitude < particleCollider)
                            {
                                dir.Normalize();
                                dir *= particleCollider;
                                //particles[i, j].pos = dir * particleCollider;
                                particles[i, j].acceleration += dir * particleCollider;
                            }
                        }
                    }
                }


                if (collision)
                    CheckCollision(particles[i, j]);
                else
                    collision = CheckCollision(particles[i, j]);

                if (!particles[i, j].posLocked && !particles[i, j].moving)
                {
                    VerletTimeStep(particles[i, j]);
                }
                else
                {
                    continue;
                }
            }
        }

        currentCollision = collision;

        sp.Stop();
        ticks.text = ("Time: " + sp.ElapsedTicks + " ticks");

        selectedParticleMesh.gameObject.transform.position = selectedParticle.pos;
        UpdateMesh();
        UpdateWireframe();
    }

    private void UpdateMesh()
    {
        vertices.Clear();
        for (int i = 0; i < gridSize; i++)
        {
            for (int j = 0; j < gridSize; j++)
            {
                vertices.Add(particles[i, j].pos);
            }
        }
        filter.mesh.SetVertices(vertices);
    }

    private void UpdateWireframe()
    {
        for (int i = 0; i < constraints.Count; i++)
        {
            gridLines[i].SetPosition(0, constraints[i].particle1.pos);
            gridLines[i].SetPosition(1, constraints[i].particle2.pos);
        }
    }

    private bool CheckCollision(ClothParticle p)
    {
        bool collision = false;

        foreach (ClothCollider c in colliders)
        {
            if (clothType == ClothType.spring)
            {
                if (collision)
                    c.CheckCollision(p, true, this);
                else
                    collision = c.CheckCollision(p, true, this);
            }
            else
            {
                c.CheckCollision(p, false, this);
            }
        }

        return collision;
    }

    private void VerletTimeStep(ClothParticle p)
    {
        Vector3 temp = p.pos;
        p.pos = p.pos + (p.pos - p.oldPos) * (1 - damp) + p.acceleration * Time.fixedDeltaTime;
        p.oldPos = temp;
        p.acceleration = Vector3.zero;
    }

    private Vector3 GetTriangleNormal(ClothParticle p1, ClothParticle p2, ClothParticle p3)
    {
        Vector3 pos1 = p1.pos;
        Vector3 pos2 = p2.pos;
        Vector3 pos3 = p3.pos;

        Vector3 v1 = pos2 - pos1;
        Vector3 v2 = pos3 - pos1;

        return Vector3.Cross(v1, v2);
    }

    private void AddWind(ClothParticle p1, ClothParticle p2, ClothParticle p3, Vector3 windForce)
    {

        Vector3 normal = GetTriangleNormal(p1, p2, p3);
        Vector3 dir = normal.normalized;
        Vector3 force = normal * (Vector3.Dot(dir, windForce));
        p1.acceleration += (force * (1 / p1.mass)) * Time.fixedDeltaTime;
        p2.acceleration += (force * (1 / p2.mass)) * Time.fixedDeltaTime;
        p3.acceleration += (force * (1 / p3.mass)) * Time.fixedDeltaTime;
    }

    public void ToggleWireFrame()
    {
        meshRenderer.enabled = !meshRenderer.enabled;

        foreach(LineRenderer lr in gridLines)
        {
            lr.gameObject.SetActive(!lr.gameObject.activeInHierarchy);
        }
    }
}

public class ClothSpring
{
    public ClothParticle particle1;
    public ClothParticle particle2;

    public float length;
    public float inverseLength;

    public float springTension;

    public ClothSpring(ClothParticle particle1, ClothParticle particle2, float Stiffness)
    {
        this.particle1 = particle1;
        this.particle2 = particle2;
        this.length = (particle1.pos - particle2.pos).magnitude;
        this.inverseLength = 1.0f / length;
        this.springTension = Stiffness;
    }
}

public class ClothConstraint
{
    public ClothParticle particle1;
    public ClothParticle particle2;

    public float restLegth;

    public ClothConstraint(ClothParticle particle1, ClothParticle particle2)
    {
        this.particle1 = particle1;
        this.particle2 = particle2;
        this.restLegth = (particle1.pos - particle2.pos).magnitude;
    }

    public void SatisfyConstraint()
    {
        Vector3 dir = particle2.pos - particle1.pos;
        float dist = dir.magnitude;
        Vector3 correction = dir * (1 - (restLegth / dist));
        Vector3 halfCorrection = correction * 0.5f;

        if (!particle1.posLocked && !particle1.moving)
        {
            particle1.pos += halfCorrection;
        }
        if (!particle2.posLocked && !particle2.moving)
        {

            particle2.pos -= halfCorrection;
        }
    }
}

public class ClothParticle
{
    public List<ClothConstraint> constraints = new List<ClothConstraint>();

    public bool posLocked = false;
    public bool moving = false;
    public Vector3 startPos;

    public Vector3 oldPos;
    public Vector3 pos;
    public Vector3 acceleration;

    public float radius;
    public float density;
    public float mass;

    public ClothParticle(Vector3 pos, float mass, float radius)
    {
        this.pos = pos;
        this.oldPos = pos;
        this.mass = mass;
        this.radius = radius;
    }
}
