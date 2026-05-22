using UnityEngine;

// Auteur: Timothy Chatelier, David Champagne, Michael Proulx
// Derniere date de modification: 22/05/2026
// Gere les mouvements, les sauts, le tir, l'accroupissement (crawl) et la gestion de la camera du joueur.
// Coordonne les animations de l'astronaute ainsi que les effets sonores relies aux actions.
[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(CapsuleCollider))]
public class AstronautController : MonoBehaviour
{
    private bool _controleBloque = false; // Vrai si les commandes du joueur sont gelees

    [Header("Poussee par ennemi")]
    public float forcePousseeEnnemi = 6f; // Force de l'impact appliquee au joueur
    public float dureePousseeEnnemi = 0.2f; // Duree pendant laquelle le joueur recule

    private Vector3 _vitessePoussee = Vector3.zero; // Vecteur de la force de recul actuelle
    private float _tempsPousseeRestant = 0f; // Chronometre pour la duree du recul

    [Header("Alerte Alien")]
    public float rayonAlerteAlien = 8f; // Rayon de propagation du bruit du tir
    public float delaiAlerteAlien = 0.5f; // Delai avant que l'ennemi ne reagisse au bruit
    public LayerMask masqueAlien; // Filtre pour detecter uniquement les ennemis

    [Header("Audio - Pas")]
    public AudioSource audioPas; // Source audio dediee aux bruits de pas
    public AudioClip[] sonsMarche; // Banques de sons pour la marche standard
    public AudioClip[] sonsCourse; // Banques de sons pour la course rapide
    public float volumePas = 0.6f; // Volume des pas debout

    [Header("Audio - Crawl")]
    public AudioClip[] sonsCrawl; // Banques de sons lorsque le joueur rampe
    public float volumeCrawl = 0.2f; // Volume des pas en mode crawl
    public float intervalleSonCrawl = 2.4f; // Temps entre deux sons de crawl en continu

    private float _timerSonCrawl = 0f; // Chronometre pour espacer les bruits de crawl

    [Header("Collider")]
    public float radiusDebout = 0.3f; // Largeur du collider en position debout
    public float radiusCrawl = 0.25f; // Largeur du collider en position rampee

    [Header("Audio - Viser")]
    public AudioSource audioVise; // Source audio pour le mecanisme de visee
    public AudioClip sonDebutVise; // Son joue au moment ou le joueur commence a viser
    public float volumeVise = 0.6f; // Volume du son de visee

    private bool _viseDerniereFrame = false; // Permet de detecter le premier instant de la visee

    [Header("Mouvement")]
    public float vitesseDeplacement = 5f; // Vitesse de marche normale
    public float vitesseCourse = 9f; // Vitesse de sprint
    public float forceSaut = 2f; // Puissance de l'impulsion verticale du saut

    [Header("Crawl / Accroupissement")]
    public float hauteurDebout = 1.8f; // Hauteur de la capsule debout
    public float hauteurCrawl = 0.55f; // Hauteur de la capsule rampee
    public float vitesseTransition = 8f; // Vitesse de changement de taille de la capsule

    [Header("Camera & Rotation")]
    public float sensibiliteSouris = 2f; // Vitesse de rotation de la camera
    public float limiteRegardVertical = 80f; // Angle maximum pour regarder en haut et en bas

    [Tooltip("Camera actuellement utilisee pour viser. Elle est changee par CameraController.")]
    public Transform transformCamera; // Reference vers le transform de la camera active

    [Header("Camera - Decalage droite/gauche")]
    public float cameraXNormal = 0f; // Position horizontale de la camera par defaut
    public float cameraXVise = 0.2f; // Position horizontale de la camera en mode visee

    [Header("Camera FPS")]
    [Tooltip("Parent de la camera FPS. Exemple : CameraPoint.")]
    public Transform cameraPoint; // Pivot de la camera utilise pour les transitions de hauteur

    [Tooltip("La vraie camera FPS. Exemple : Main Camera.")]
    public Transform cameraFPS; // Objet de la camera reellement deplace dans l'espace

    [Header("Camera - Debout")]
    public float hauteurCameraDebout = 1.9f; // Hauteur camera au repos debout
    public float cameraZDebout = 0.2f; // Position avant/arriere debout

    [Header("Camera - Marche")]
    public float hauteurCameraMarche = 1.9f; // Hauteur camera en marche standard
    public float cameraZMarche = 0.2f; // Position avant/arriere en marche standard

    [Header("Camera - Course")]
    public float hauteurCameraCourse = 1.9f; // Hauteur camera pendant la course
    public float cameraZCourse = 0.5f; // Position avant/arriere pendant la course

    [Header("Camera - Vise")]
    public float hauteurCameraVise = 1.75f; // Hauteur camera en mode visee
    public float cameraZVise = 0.1f; // Position avant/arriere en mode visee

    [Header("Camera - Interaction")]
    public float hauteurCameraUse = 1.7f; // Hauteur camera pendant une interaction
    public float cameraZUse = 0.2f; // Position avant/arriere pendant une interaction

    [Header("Camera - Crawl")]
    public float hauteurCameraCrawl = 0.55f; // Hauteur camera au ras du sol
    public float cameraZCrawl = 0.6f; // Position avant/arriere en mode crawl

    public float vitesseCameraCrawl = 8f; // Vitesse de transition de la position camera

    [Header("Detection du sol")]
    public Transform pointSol; // Point d'origine de la sphere de verification du sol
    public float rayonSol = 0.3f; // Taille du rayon de detection du sol
    public LayerMask masqueSol; // Couches considerees comme du sol solide

    [Header("Actions")]
    public float dureeUse = 1.5f; // Duree de blocage pendant une interaction F

    [Header("Tir")]
    [Tooltip("Point de depart du tir. Mets-le sur le gun, pas sous la camera.")]
    public Transform pointTir; // Emplacement de la bouche de l'arme

    public ParticleSystem effetTir; // Flash de lumiere au bout du canon
    public ParticleSystem effetSpark; // Etincelles projetees au moment du tir

    public AudioSource audioTir; // Source audio dediee aux coups de feu
    public AudioClip sonTir; // Effet sonore de la détonation

    public float delaiEntreTirs = 0.4f; // Cadence de tir minimale
    private float _tempsProchainTir = 0f; // Timestamp interne pour autoriser le prochain tir

    public float porteeTir = 100f; // Distance maximale des projectiles
    public float degatsParTir = 25f; // Quantite de degats infliges a l'ennemi
    public LayerMask masqueTir; // Filtre des objets pouvant recevoir des impacts de balles
    public GameObject effetImpact; // Prefab d'impact (poussiere, trous de balle) cree sur la cible

    [Header("Tir - Visuel")]
    public float dureeTracer = 0.05f; // Temps d'affichage de la ligne laser du tir
    public UnityEngine.UI.Image[] partiesReticule; // Composants UI qui forment le viseur a l'ecran

    [Header("Reticule")]
    public LayerMask masqueEnnemi; // Couche utilisee pour verifier si le viseur survole un alien

    // References aux composants locaux recuperees au demarrage
    private Rigidbody _rb;
    private Animator _anim;
    private CapsuleCollider _capsule;
    private LineRenderer _tracer;

    // Etats de controle internes du personnage
    private bool _estAccroupi = false;
    private bool _estAuSol;
    private bool _enCoursUse = false;
    private bool _dansConduitVent = false;
    private bool _estEnTrainDeViser = false;

    private float _rotationVerticale = 0f; // Angle de rotation de haut en bas
    private float _rotationHorizontale = 0f; // Angle de rotation de gauche a droite
    private bool _modeTPS = false; // Indique si le jeu est en vue a la troisieme personne

    // Mots-cles haches pour optimiser les performances de l'Animator
    private static readonly int P_Crouch = Animator.StringToHash("Crouch");
    private static readonly int P_Speed = Animator.StringToHash("Speed");
    private static readonly int P_CrawlAnimSpeed = Animator.StringToHash("CrawlAnimSpeed");
    private static readonly int P_Grounded = Animator.StringToHash("Grounded");
    private static readonly int P_Jump = Animator.StringToHash("Jump");
    private static readonly int P_Shoot = Animator.StringToHash("Shoot");
    private static readonly int P_Use = Animator.StringToHash("Use");
    private static readonly int P_Aiming = Animator.StringToHash("Aiming");
    private static readonly int P_StopUse = Animator.StringToHash("StopUse");

    // Initialise les composants, verrouille le curseur et configure le tracer de tir
    void Start()
    {
        _rb = GetComponent<Rigidbody>();
        _anim = GetComponent<Animator>();
        _capsule = GetComponent<CapsuleCollider>();

        // Configuration de la physique du Rigidbody
        _rb.constraints = RigidbodyConstraints.FreezeRotation;
        _rb.interpolation = RigidbodyInterpolation.Interpolate;
        _rb.collisionDetectionMode = CollisionDetectionMode.ContinuousDynamic;
        _rb.useGravity = true;

        int defaultLayer = LayerMask.NameToLayer("Default");
        masqueAlien = LayerMask.GetMask("Ennemy");

        if (defaultLayer != -1)
        {
            masqueTir |= (1 << defaultLayer);
        }

        _rotationHorizontale = transform.eulerAngles.y;

        // Cache et bloque le curseur de la souris au centre de l'ecran
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        // Recherche automatique de la camera si non assignee
        if (transformCamera == null)
        {
            Camera cam = GetComponentInChildren<Camera>();
            if (cam != null)
            {
                transformCamera = cam.transform;
            }
            else
            {
                Debug.LogWarning("[AstronautController] Aucune camera trouvee.");
            }
        }

        if (cameraFPS == null && transformCamera != null)
        {
            cameraFPS = transformCamera;
        }

        // Configuration du composant LineRenderer pour l'effet visuel des tirs
        if (pointTir != null)
        {
            _tracer = pointTir.GetComponent<LineRenderer>();
            if (_tracer == null)
            {
                _tracer = pointTir.GetComponentInChildren<LineRenderer>();
            }
        }

        if (_tracer != null)
        {
            _tracer.enabled = false;
            _tracer.useWorldSpace = true;
            _tracer.positionCount = 2;
            _tracer.startWidth = 0.02f;
            _tracer.endWidth = 0.02f;
        }
    }

    // Accesseur public pour savoir si l'astronaute est en train de viser
    public bool EstEnTrainDeViser()
    {
        return _estEnTrainDeViser;
    }

    // Accesseur public pour verifier si le joueur utilise la camera TPS
    public bool EstModeTPS()
    {
        return _modeTPS;
    }

    // Boucle principale : execute les verifications d'etats, la rotation et met a jour le visuel de la frame
    void Update()
    {
        // Force l'arret complet si le controle est bloque (cinematique, mort, etc.)
        if (_controleBloque)
        {
            if (_rb != null)
            {
                _rb.linearVelocity = Vector3.zero;
                _rb.angularVelocity = Vector3.zero;
            }

            if (_anim != null)
            {
                _anim.SetFloat(P_Speed, 0f);
                _anim.SetFloat(P_CrawlAnimSpeed, 0f);
                _anim.SetBool(P_Aiming, false);
                _anim.SetBool(P_Crouch, false);
            }

            _estEnTrainDeViser = false;
            _enCoursUse = false;
            return;
        }

        VerifierSol();
        GererRotationCamera();
        AppliquerRotationJoueur();
        GererActions();
        GererCrawl();
        MettreAJourCameraFPS();
        MettreAJourAnimations();
        MettreAJourReticule();
        GererSonCrawl();
    }

    // Traite les deplacements physiques a intervalles de temps constants
    void FixedUpdate()
    {
        if (_controleBloque)
        {
            if (_rb != null)
            {
                _rb.linearVelocity = Vector3.zero;
            }
            return;
        }

        GererMouvement();
    }

    // Determine si le joueur touche le sol via un test de sphere (Physics.CheckSphere)
    void VerifierSol()
    {
        Vector3 origineSol;

        if (pointSol != null)
        {
            origineSol = pointSol.position;
        }
        else if (_capsule != null)
        {
            origineSol = transform.position + Vector3.down * (_capsule.height / 2f - _capsule.radius * 0.5f);
        }
        else
        {
            origineSol = transform.position + Vector3.down * 0.9f;
        }

        _estAuSol = Physics.CheckSphere(
            origineSol,
            rayonSol,
            masqueSol,
            QueryTriggerInteraction.Ignore
        );
    }

    // Calcule l'orientation verticale et horizontale selon les mouvements de la souris
    void GererRotationCamera()
    {
        float sourisX = Input.GetAxisRaw("Mouse X") * sensibiliteSouris;
        float sourisY = Input.GetAxisRaw("Mouse Y") * sensibiliteSouris;

        _rotationHorizontale += sourisX;
        _rotationVerticale -= sourisY;
        _rotationVerticale = Mathf.Clamp(_rotationVerticale, -limiteRegardVertical, limiteRegardVertical);

        // Applique la rotation verticale directement a la camera en premiere personne
        if (!_modeTPS && transformCamera != null)
        {
            transformCamera.localRotation = Quaternion.Euler(_rotationVerticale, 0f, 0f);
        }
    }

    // Verrouille ou debloque les actions du joueur et reinitialise tous les parametres de mouvement
    public void BloquerControle(bool bloque)
    {
        _controleBloque = bloque;

        if (_rb != null)
        {
            _rb.linearVelocity = Vector3.zero;
            _rb.angularVelocity = Vector3.zero;
        }

        if (_anim != null)
        {
            _anim.SetFloat(P_Speed, 0f);
            _anim.SetFloat(P_CrawlAnimSpeed, 0f);
            _anim.SetBool(P_Aiming, false);
            _anim.SetBool(P_Crouch, false);

            _anim.ResetTrigger(P_Shoot);
            _anim.ResetTrigger(P_Use);
            _anim.ResetTrigger(P_Jump);
            _anim.ResetTrigger(P_StopUse);
        }

        _estEnTrainDeViser = false;
        _enCoursUse = false;
        _tempsPousseeRestant = 0f;
        _vitessePoussee = Vector3.zero;
    }

    // Applique l'orientation horizontale calculee au corps entier du joueur
    void AppliquerRotationJoueur()
    {
        Quaternion rotationCible = Quaternion.Euler(0f, _rotationHorizontale, 0f);
        transform.rotation = rotationCible;
    }

    // Execute les deplacements au sol du joueur selon les axes d'inputs verticaux et horizontaux
    void GererMouvement()
    {
        // Stoppe les mouvements au sol si le personnage interagit ou s'il vise
        if (_enCoursUse || _estEnTrainDeViser)
        {
            _rb.linearVelocity = new Vector3(0f, _rb.linearVelocity.y, 0f);
            return;
        }

        // Applique l'inertie du recul provoque par une attaque ennemie
        if (_tempsPousseeRestant > 0f)
        {
            _tempsPousseeRestant -= Time.fixedDeltaTime;
            _rb.linearVelocity = new Vector3(_vitessePoussee.x, _rb.linearVelocity.y, _vitessePoussee.z);
            return;
        }

        float h = Input.GetAxisRaw("Horizontal");
        float v = Input.GetAxisRaw("Vertical");

        bool court = Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift);
        float vitesse;

        if (_estAccroupi)
        {
            vitesse = vitesseDeplacement * 0.45f; // Reduit la vitesse en mode crawl
        }
        else
        {
            vitesse = court ? vitesseCourse : vitesseDeplacement;
        }

        Vector3 direction = (transform.right * h + transform.forward * v).normalized;
        Vector3 cible = direction * vitesse;

        _rb.linearVelocity = new Vector3(cible.x, _rb.linearVelocity.y, cible.z);
    }

    // Gere l'impulsion de saut lorsque le joueur appuie sur la barre Espace
    void GererSaut()
    {
        if (_enCoursUse || _estEnTrainDeViser)
        {
            return;
        }

        if (Input.GetButtonDown("Jump") && _estAuSol && !_estAccroupi)
        {
            _rb.linearVelocity = new Vector3(_rb.linearVelocity.x, 0f, _rb.linearVelocity.z);
            _rb.AddForce(Vector3.up * forceSaut, ForceMode.Impulse);

            if (_anim != null)
            {
                _anim.SetTrigger(P_Jump);
            }
        }
    }

    // Joue en boucle les sons de crawl a intervalles reguliers si le joueur avance a croupi
    void GererSonCrawl()
    {
        if (audioPas == null)
        {
            return;
        }

        if (!_estAccroupi || !_estAuSol || _enCoursUse || _estEnTrainDeViser)
        {
            _timerSonCrawl = 0f;
            return;
        }

        float h = Input.GetAxisRaw("Horizontal");
        float v = Input.GetAxisRaw("Vertical");
        bool bouge = Mathf.Abs(h) > 0.1f || Mathf.Abs(v) > 0.1f;

        if (!bouge)
        {
            _timerSonCrawl = 0f;
            return;
        }

        _timerSonCrawl -= Time.deltaTime;
        if (_timerSonCrawl > 0f)
        {
            return;
        }

        if (sonsCrawl == null || sonsCrawl.Length == 0)
        {
            return;
        }

        int index = Random.Range(0, sonsCrawl.Length);
        AudioClip sonChoisi = sonsCrawl[index];

        if (sonChoisi != null)
        {
            audioPas.PlayOneShot(sonChoisi, volumeCrawl);
        }

        _timerSonCrawl = intervalleSonCrawl;
    }

    // Alterne entre l'etat debout et l'etat rampe (touche Q) et adapte dynamiquement la taille du collider
    void GererCrawl()
    {
        if (Input.GetKeyDown(KeyCode.Q))
        {
            if (_enCoursUse || _estEnTrainDeViser)
            {
                return;
            }

            // Empeche de se relever si le joueur se trouve force dans un conduit de ventilation
            if (_dansConduitVent && _estAccroupi)
            {
                return;
            }

            _estAccroupi = !_estAccroupi;

            if (_anim != null)
            {
                _anim.SetBool(P_Crouch, _estAccroupi);
            }
        }

        // Interpolation de la hauteur et du rayon du collider pour eviter les changements brusques
        if (_capsule != null)
        {
            float hauteurCible = _estAccroupi ? hauteurCrawl : hauteurDebout;
            float radiusCible = _estAccroupi ? radiusCrawl : radiusDebout;

            _capsule.height = Mathf.Lerp(_capsule.height, hauteurCible, Time.deltaTime * vitesseTransition);
            _capsule.radius = Mathf.Lerp(_capsule.radius, radiusCible, Time.deltaTime * vitesseTransition);
            _capsule.center = new Vector3(0f, _capsule.height / 2f, 0f);
        }
    }

    // Ajuste en continu la position locale de la camera pour simuler l'effet de deplacement (marche, course, tir, crawl)
    void MettreAJourCameraFPS()
    {
        if (cameraPoint == null || cameraFPS == null)
        {
            return;
        }

        float hauteurCible = hauteurCameraDebout;
        float zCible = cameraZDebout;
        float xCible = cameraXNormal;

        float h = Input.GetAxisRaw("Horizontal");
        float v = Input.GetAxisRaw("Vertical");

        bool bouge = Mathf.Abs(h) > 0.1f || Mathf.Abs(v) > 0.1f;
        bool court = Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift);

        // Selection des cibles de placement de la camera selon l'action en cours
        if (_estAccroupi)
        {
            hauteurCible = hauteurCameraCrawl;
            zCible = cameraZCrawl;
            xCible = cameraXNormal;
        }
        else if (_enCoursUse)
        {
            hauteurCible = hauteurCameraUse;
            zCible = cameraZUse;
            xCible = cameraXNormal;
        }
        else if (_estEnTrainDeViser)
        {
            hauteurCible = hauteurCameraVise;
            zCible = cameraZVise;
            xCible = cameraXVise;
        }
        else if (bouge && court)
        {
            hauteurCible = hauteurCameraCourse;
            zCible = cameraZCourse;
            xCible = cameraXNormal;
        }
        else if (bouge)
        {
            hauteurCible = hauteurCameraMarche;
            zCible = cameraZMarche;
            xCible = cameraXNormal;
        }

        // Application fluide des positions de la camera (effet Lerp)
        Vector3 positionPoint = cameraPoint.localPosition;
        positionPoint.y = Mathf.Lerp(positionPoint.y, hauteurCible, Time.deltaTime * vitesseCameraCrawl);
        cameraPoint.localPosition = positionPoint;

        Vector3 positionCamera = cameraFPS.localPosition;
        positionCamera.x = Mathf.Lerp(positionCamera.x, xCible, Time.deltaTime * vitesseCameraCrawl);
        positionCamera.y = 0f;
        positionCamera.z = Mathf.Lerp(positionCamera.z, zCible, Time.deltaTime * vitesseCameraCrawl);
        cameraFPS.localPosition = positionCamera;
    }

    // Distribue les etats de tir, de visee et d'interaction en fonction des entrees souris et clavier du joueur
    void GererActions()
    {
        if (_enCoursUse)
        {
            _estEnTrainDeViser = false;
            _viseDerniereFrame = false;

            if (_anim != null)
            {
                _anim.SetBool(P_Aiming, false);
            }
            return;
        }

        // Le joueur peut viser seulement s'il est debout.
        bool viseMaintenant = Input.GetMouseButton(1) && !_estAccroupi;

        if (viseMaintenant && !_viseDerniereFrame)
        {
            if (audioVise != null && sonDebutVise != null)
            {
                audioVise.PlayOneShot(sonDebutVise, volumeVise);
            }
        }

        _estEnTrainDeViser = viseMaintenant;
        _viseDerniereFrame = viseMaintenant;

        // Tir seulement si le joueur vise et respecte le delai de cadence
        if (Input.GetMouseButtonDown(0))
        {
            if (!_estEnTrainDeViser)
            {
                return;
            }

            if (Time.time < _tempsProchainTir)
            {
                return;
            }

            _tempsProchainTir = Time.time + delaiEntreTirs;

            if (_anim != null)
            {
                _anim.SetTrigger(P_Shoot);
            }

            Tirer();
        }

        // Interaction avec F.
        if (Input.GetKeyDown(KeyCode.F))
        {
            if (_estAccroupi || _estEnTrainDeViser)
            {
                return;
            }

            if (_anim != null)
            {
                _anim.SetTrigger(P_Use);
            }

            StartCoroutine(UseCooldown());
        }

        if (_anim != null)
        {
            _anim.SetBool(P_Aiming, _estEnTrainDeViser);
        }
    }

    // Utilise un lancer de rayon (Raycast) pour detecter les cibles touchees et appliquer les degats
    void Tirer()
    {
        if (pointTir == null || transformCamera == null)
        {
            return;
        }
        if (audioTir != null && sonTir != null)
        {
            audioTir.PlayOneShot(sonTir);
        }
        if (effetTir != null)
        {
            effetTir.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
            effetTir.Emit(10);
        }
        else
        {
            Debug.LogWarning("effetTir n'est pas assigne dans l'Inspector.");
        }
        if (effetSpark != null)
        {
            effetSpark.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
            effetSpark.Emit(15);
        }

        Ray rayCamera = new Ray(transformCamera.position, transformCamera.forward);
        Vector3 cible;

        // Premier Raycast depuis la camera pour savoir precisement ce que regarde le joueur
        if (Physics.Raycast(rayCamera, out RaycastHit hitVisee, porteeTir, masqueTir))
        {
            cible = hitVisee.point;
        }
        else
        {
            cible = rayCamera.GetPoint(porteeTir);
        }

        // Deuxieme Raycast partant du canon vers le point de visee pour simuler le projectile reel
        Vector3 directionTir = (cible - pointTir.position).normalized;
        Vector3 pointImpact = pointTir.position + directionTir * porteeTir;

        if (Physics.Raycast(pointTir.position, directionTir, out RaycastHit hit, porteeTir, masqueTir))
        {
            pointImpact = hit.point;

            // Transmission des degats aux scripts de sante des aliens touches
            EnnemyHealth ennemi = hit.collider.GetComponentInParent<EnnemyHealth>();
            if (ennemi != null)
            {
                ennemi.PrendreDegats(degatsParTir);

                AlienPatrol alienPatrol = hit.collider.GetComponentInParent<AlienPatrol>();
                if (alienPatrol != null)
                {
                    alienPatrol.ArreterCourtement();
                }
            }

            if (effetImpact != null)
            {
                Instantiate(effetImpact, hit.point, Quaternion.LookRotation(hit.normal));
            }
            AlerterAliensAutour(hit.point);

            Debug.DrawRay(pointTir.position, directionTir * hit.distance, Color.red, 1f);
        }
        else
        {
            Debug.DrawRay(pointTir.position, directionTir * porteeTir, Color.yellow, 1f);
        }

        if (_tracer != null)
        {
            StartCoroutine(AfficherTracer(pointTir.position, pointImpact));
        }
    }

    // Alterne la couleur des composants UI du reticule si un alien est survole par la camera
    void MettreAJourReticule()
    {
        if (transformCamera == null)
        {
            return;
        }

        bool surEnnemi = Physics.Raycast(
            transformCamera.position,
            transformCamera.forward,
            porteeTir,
            masqueEnnemi
        );

        Color couleur = surEnnemi ? Color.red : Color.white;

        foreach (var partie in partiesReticule)
        {
            if (partie != null)
            {
                partie.color = couleur;
            }
        }
    }

    // Definit une sphere d'alerte autour de l'impact pour attirer l'agro des monstres proches
    void AlerterAliensAutour(Vector3 positionImpact)
    {
        Collider[] aliensTouches = Physics.OverlapSphere(
            positionImpact,
            rayonAlerteAlien,
            masqueAlien
        );

        foreach (Collider alienCollider in aliensTouches)
        {
            AlienPatrol alien = alienCollider.GetComponentInParent<AlienPatrol>();
            if (alien != null)
            {
                alien.DeclencherAgroAvecDelai(transform, delaiAlerteAlien);
            }
        }
    }

    // Coroutine chargee d'activer puis d'eteindre la ligne de visee laser pour materialiser le projectile
    private System.Collections.IEnumerator AfficherTracer(Vector3 debut, Vector3 fin)
    {
        if (_tracer == null)
        {
            yield break;
        }

        _tracer.positionCount = 2;
        _tracer.startWidth = 0.02f;
        _tracer.endWidth = 0.02f;
        _tracer.enabled = true;

        _tracer.SetPosition(0, debut);
        _tracer.SetPosition(1, fin);

        yield return new WaitForSeconds(dureeTracer);

        _tracer.enabled = false;
    }

    // Bloque temporairement les deplacements du joueur pendant qu'il execute une action F
    private System.Collections.IEnumerator UseCooldown()
    {
        _enCoursUse = true;
        _rb.linearVelocity = new Vector3(0f, _rb.linearVelocity.y, 0f);

        yield return new WaitForSeconds(dureeUse);

        if (_anim != null)
        {
            _anim.SetTrigger(P_StopUse);
        }

        _enCoursUse = false;
    }

    // Injecte et synchronise les variables de vitesse et d'etat calcules dans le composant Animator
    void MettreAJourAnimations()
    {
        if (_anim == null)
        {
            return;
        }

        float speedAnim = 0f;
        float crawlAnimSpeed = 0f;

        if (!_enCoursUse)
        {
            float h = Input.GetAxisRaw("Horizontal");
            float v = Input.GetAxisRaw("Vertical");

            float inputMouvement = new Vector3(h, 0f, v).magnitude;
            inputMouvement = Mathf.Clamp01(inputMouvement);

            bool court = Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift);

            if (inputMouvement > 0.1f)
            {
                if (court && !_estAccroupi)
                {
                    speedAnim = 1f; // Vitesse d'animation pour courir
                }
                else
                {
                    speedAnim = 0.5f; // Vitesse d'animation pour marcher
                }
            }

            if (_estAccroupi)
            {
                crawlAnimSpeed = inputMouvement;
            }
            else
            {
                crawlAnimSpeed = 1f;
            }
        }

        if (_enCoursUse || _estEnTrainDeViser)
        {
            speedAnim = 0f;
            crawlAnimSpeed = 0f;
        }

        _anim.SetFloat(P_Speed, speedAnim, 0.1f, Time.deltaTime);
        _anim.SetBool(P_Grounded, _estAuSol);
        _anim.SetBool(P_Crouch, _estAccroupi);
        _anim.SetFloat(P_CrawlAnimSpeed, crawlAnimSpeed);
    }

    // Declenche par un Animation Event via les clips de marche/course pour jouer les sons de pas correspondants
    public void FootStep()
    {
        if (audioPas == null)
        {
            return;
        }

        if (!_estAuSol || _enCoursUse || _estEnTrainDeViser)
        {
            return;
        }

        float h = Input.GetAxisRaw("Horizontal");
        float v = Input.GetAxisRaw("Vertical");
        bool bouge = Mathf.Abs(h) > 0.1f || Mathf.Abs(v) > 0.1f;

        if (!bouge)
        {
            return;
        }

        if (_estAccroupi)
        {
            if (sonsCrawl == null || sonsCrawl.Length == 0)
            {
                return;
            }

            int indexCrawl = Random.Range(0, sonsCrawl.Length);
            AudioClip sonCrawl = sonsCrawl[indexCrawl];

            if (sonCrawl != null)
            {
                audioPas.PlayOneShot(sonCrawl, volumeCrawl);
            }
            return;
        }

        bool court = Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift);
        AudioClip[] listeSons = court ? sonsCourse : sonsMarche;

        if (listeSons == null || listeSons.Length == 0)
        {
            return;
        }

        int index = Random.Range(0, listeSons.Length);
        AudioClip sonChoisi = listeSons[index];

        if (sonChoisi != null)
        {
            audioPas.PlayOneShot(sonChoisi, volumePas);
        }
    }

    // Accesseur public pour connaitre la position accroupie actuelle du personnage
    public bool EstAccroupi()
    {
        return _estAccroupi;
    }

    // Calcule la trajectoire de poussee appliquee lorsque le joueur encaisse un coup
    public void RecevoirPoussee(Vector3 positionSource)
    {
        Vector3 direction = transform.position - positionSource;
        direction.y = 0f;
        direction.Normalize();

        _vitessePoussee = direction * forcePousseeEnnemi;
        _tempsPousseeRestant = dureePousseeEnnemi;
    }

    // Met a jour les variables de configuration de camera depuis le systeme principal de gestion camera
    public void ChangerCameraActive(Transform nouvelleCamera, bool estTPS)
    {
        transformCamera = nouvelleCamera;
        _modeTPS = estTPS;
    }

    // Verifie l'entree dans des zones triggers specifiques comme les conduits de ventilation
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Vent"))
        {
            _dansConduitVent = true;
        }
    }

    // Verifie la sortie des zones triggers specifiques
    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Vent"))
        {
            _dansConduitVent = false;
        }
    }

    // Dessine dans la scene Unity l'indicateur spherique vert ou rouge servant a deboguer la detection du sol
    void OnDrawGizmosSelected()
    {
        Gizmos.color = _estAuSol ? Color.green : Color.red;
        Vector3 pos;

        if (pointSol != null)
        {
            pos = pointSol.position;
        }
        else
        {
            pos = transform.position + Vector3.down * 0.9f;
        }

        Gizmos.DrawWireSphere(pos, rayonSol);
    }
}