const API_URL_BASE = location.host;

const API_URLS = {
    VALIDATE_CREDENTIALS:   '/Seguridad/Home/ValidaCredenciales',
    VALIDATE_USER:          '/Seguridad/Home/ValidaSoloUsuario',
    EVALUATE_FB:            '/Seguridad/Home/EvaluaFace',
    EVALUATE_JWT:           '/Seguridad/Home/EvaluaJWT',
    PROCESS_FB:             '/Seguridad/Home/ProcesaFace',
    PROCESS_JWT:            '/Seguridad/Home/ProcesaJWT',
    NET_USERS:              '/Seguridad/Home/Consulta_SSO_usuarioRed',
    DONT_SHOW_NETPAGE:      '/Seguridad/Home/Consulta_NoMostrar',
    UPDATE_SHOW_NETPAGE:    '/Seguridad/Home/UpdNoMostrar',
    UNLINK_SOCIAL_ACCOUNT:  '/Seguridad/Home/BorraEn_SSO_usuarioRed',
    PROCESS_CODE_EMAIL:     '/Proceso/Home/CodeSend',
    PROCESS_CODE_SMS:       '/Proceso/Home/CodeSendSMS',
    EVALUATE_CODE:          '/Proceso/Home/CodeEval',
    CHANGE_PASSWORD:        '/Proceso/Home/ChangePasswordAD',
};

const UI_URLS = {
    INDEX:                 '/',
    LOGIN:                 '/Home/login',
    SOCIAL_NETWORKS:       '/Home/Redes',
    RECOVER_PASS:          '/Home/recoverPass',
    RECOVER_METHOD:        '/Home/recoverMethod',
    RECOVER_CONFIRM_SMS:   '/Home/recoverConfirmSms',
    RECOVER_CONFIRM_EMAIL: '/Home/recoverConfirmEmail',
    VINCULATE_ACCOUNTS:    '/Home/vinculateAccounts',
    NEXT_PAGE:             '/Home/Contact',
    RECOVER_VERIFICATION:  '/Home/recoverVerification',
    CREATE_PASS:           '/Home/createPassword',
    UPDATED_PASSWORD:      '/Home/updatedPassword',
};

const UI_ERRORS = {
    ERROR: 'Error',
    SERVER_ERROR: 'Error en el servidor:',
    UNKNOWN_ERROR: 'Error desconocido',
    SERVER_NOT_RESPOND: 'Servidor no responde.',
    FORMS: {
        CORRECT_ERRORS: 'Corregir los errores:',
        INPUTS_REQUIRED: 'Ingrese usuario y contraseña',
        USER_REQUIRED: 'El campo usuario es requerido',
        PASSWORD_REQUIRED: 'El campo contraseña es requerido',
    },
    LOGIN: {
        USER_PASS_INCORRECT: 'Error en la autenticación',
        REMIND_FAILED_ATTEMPTS: 'Recuerde que luego de algunos intentos fallidos su cuenta se bloqueará.'
    },
    RECOVER: {
        NO_PHONE_REGISTERED: 'El usuario no tiene registrado un numero celular',
        NO_EMAIL_REGISTERED: 'El usuario no tiene registrado un email',
    }
};

const OPTION_SMS = 'sms';
const OPTION_EMAIL = 'email';
