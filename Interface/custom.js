window.onload = function() {
    lucide.createIcons();
};

// --- State Management ---
let isSignUp = false;
let isLoading = false;

// --- DOM Elements ---
const authModalEl = document.getElementById('authModal');
const authForm = document.getElementById('authForm');
const submitButton = document.getElementById('submitButton');
const cardTitle = document.getElementById('card-title');
const cardDescription = document.getElementById('card-description');
const toggleText = document.getElementById('toggleText');
const signUpFields = document.getElementById('signUpFields');
const roleSelect = document.getElementById('role');
const toastContainer = document.getElementById('toastContainer');

// ERD-based elements
const firstNameInput = document.getElementById('first_name');
const lastNameInput = document.getElementById('last_name');
const genderSelect = document.getElementById('gender');
const phoneInput = document.getElementById('phone');

const charityFieldsDiv = document.getElementById('charityFields');
const charNameInput = document.getElementById('char_name');
const charCrInput = document.getElementById('char_cr'); // Commercial Registration

const locationGroupDiv = document.getElementById('locationGroup');
const locationInput = document.getElementById('location');
const locationLabel = document.getElementById('locationLabel');

// --- Utility Functions ---

/**
 * Simple Toast/Notification simulation using Bootstrap's component structure.
 */
function showToast(title, description, variant = 'bg-primary') {
    const toastHtml = `
                <div class="toast align-items-center ${variant} text-white border-0" role="alert" aria-live="assertive" aria-atomic="true">
                    <div class="d-flex">
                        <div class="toast-body fw-bold">
                            ${title}
                        </div>
                        <button type="button" class="btn-close btn-close-white me-2 m-auto" data-bs-dismiss="toast" aria-label="Close"></button>
                    </div>
                </div>
            `;

    const wrapper = document.createElement('div');
    wrapper.innerHTML = toastHtml;
    toastContainer.appendChild(wrapper.firstChild);

    const toastEl = toastContainer.lastElementChild;
    const toast = new bootstrap.Toast(toastEl);
    toast.show();

    // Clean up DOM after toast is hidden
    toastEl.addEventListener('hidden.bs.toast', () => {
        toastEl.remove();
    });
}

/**
 * Updates the UI state (titles, buttons, fields visibility).
 */
function updateUI() {
    // Update Card Header
    cardTitle.textContent = isSignUp ? 'انضم إلى مجتمعنا' : 'مرحباً بعودتك';
    cardDescription.textContent = isSignUp
        ? 'أنشئ حسابك لبدء ربط المزارع بالمجتمعات'
        : 'سجّل الدخول إلى حسابك للمتابعة';

    // Update Toggle Link
    toggleText.innerHTML = isSignUp
        ? `لديك حساب بالفعل؟ <a href="#" class="fresh-green fw-bold text-decoration-none" id="toggleAuthMode">تسجيل الدخول</a>`
        : `ليس لديك حساب؟ <a href="#" class="fresh-green fw-bold text-decoration-none" id="toggleAuthMode">إنشاء حساب</a>`;

    // Reattach event listener to the new toggle link (must be done after innerHTML change)
    document.getElementById('toggleAuthMode').addEventListener('click', toggleAuthMode);

    // Update Button Text
    submitButton.textContent = isLoading ? 'الرجاء الانتظار...' : (isSignUp ? 'إنشاء حساب' : 'تسجيل الدخول');
    submitButton.disabled = isLoading;

    // Toggle Sign Up Fields container
    signUpFields.style.display = isSignUp ? 'block' : 'none';

    // Update conditional fields visibility and requirements
    updateConditionalFields();
}

/**
 * Updates the visibility and required status of conditional fields based on the selected role.
 */
function updateConditionalFields() {
    const role = roleSelect.value;

    // 1. Reset all conditional field requirements/visibility
    firstNameInput.required = false;
    lastNameInput.required = false;
    genderSelect.required = false;
    locationInput.required = false;
    charNameInput.required = false;
    charCrInput.required = false;
    phoneInput.required = false;

    // Default visibility
    charityFieldsDiv.style.display = 'none';
    locationGroupDiv.style.display = 'none';

    if (isSignUp) {
        // Phone is required for all sign-ups based on the User entity
        phoneInput.required = true;

        // First Name / Last Name / Gender are required for individual accounts (Donor/Farmer)
        if (role === 'donor' || role === 'farmer') {
            firstNameInput.required = true;
            lastNameInput.required = true;
            genderSelect.required = true;

            locationGroupDiv.style.display = 'block';
        }

        // Farmer specific fields
        if (role === 'farmer') {
            locationInput.required = true;
            locationLabel.textContent = 'موقع المزرعة (مطلوب)'; // Far_Location
        } else if (role === 'donor') {
            locationInput.required = false;
            locationLabel.textContent = 'العنوان (اختياري)';
        }

        // Charity specific fields
        if (role === 'charity') {
            // Show charity fields
            charityFieldsDiv.style.display = 'block';
            locationGroupDiv.style.display = 'block';

            // Charity specific required fields (Char_Name, Char_Location)
            charNameInput.required = true;
            locationInput.required = true;
            locationLabel.textContent = 'موقع الجمعية (مطلوب)'; // Char_Location
        }
    }
}

/**
 * Toggles between Sign In and Sign Up mode.
 */
function toggleAuthMode(e) {
    e.preventDefault();
    isSignUp = !isSignUp;
    // Reset form for clean state transition and remove validation classes
    authForm.reset();
    authForm.classList.remove('was-validated');

    // Ensure email/password are marked as required on toggle (always required)
    document.getElementById('email').required = true;
    document.getElementById('password').required = true;

    // Update UI which calls updateConditionalFields to set conditional requirements
    updateUI();
}

/**
 * Sets initial auth mode based on the button that triggered the modal.
 */
function setInitialAuthMode(mode) {
    const newIsSignUp = mode === 'signUp';
    if (newIsSignUp !== isSignUp) {
        isSignUp = newIsSignUp;
        authForm.reset();
        authForm.classList.remove('was-validated');
        updateUI();
    }
}

// --- Mock API Functions ---

async function mockSignIn(email, password) {
    return new Promise((resolve, reject) => {
        setTimeout(() => {
            if (email && password) {
                if (email === "error@example.com") {
                    reject(new Error("هذا الحساب محظور مؤقتاً."));
                } else {
                    resolve({ success: true, user: { id: 'mock-user-123' } });
                }
            } else {
                reject(new Error("البريد الإلكتروني وكلمة المرور مطلوبان."));
            }
        }, 1500);
    });
}

async function mockSignUp(data) {
    // Check essential ERD fields based on User_Type
    if (data.role === 'charity' && (!data.char_name || !data.location)) {
        return new Promise((resolve, reject) => {
            reject(new Error("اسم الجمعية وموقعها مطلوبان للتسجيل."));
        });
    } else if ((data.role === 'donor' || data.role === 'farmer') && (!data.first_name || !data.last_name || !data.gender)) {
        return new Promise((resolve, reject) => {
            reject(new Error("الاسم الأول واسم العائلة والنوع مطلوبان للتسجيل."));
        });
    }

    return new Promise((resolve, reject) => {
        setTimeout(() => {
            if (data.email === "exists@example.com") {
                reject(new Error("البريد الإلكتروني مسجل بالفعل."));
            } else {
                // Log structured data as if sending to backend for all entities
                console.log("Mock data payload for backend (User and entity tables):", {
                    User: {
                        email: data.email,
                        phone: data.phone,
                        type: data.role
                    },
                    Entity: data.role === 'donor' ? {
                        first_name: data.first_name,
                        last_name: data.last_name,
                        gender: data.gender,
                    } : data.role === 'farmer' ? {
                        first_name: data.first_name,
                        last_name: data.last_name,
                        gender: data.gender,
                        location: data.location,
                    } : { // Charity
                        char_name: data.char_name,
                        char_cr: data.char_cr,
                        location: data.location,
                    }
                });

                // Mock success
                resolve({ success: true, user: { id: 'mock-new-user-456' } });
            }
        }, 2000);
    });
}

/**
 * Main form submission handler.
 */
async function handleSubmit(e) {
    e.preventDefault();

    // Basic Bootstrap validation check
    if (!authForm.checkValidity()) {
        e.stopPropagation();
        authForm.classList.add('was-validated');
        return;
    }

    isLoading = true;
    updateUI();

    const formData = new FormData(authForm);
    const data = Object.fromEntries(formData.entries());

    try {
        if (isSignUp) {
            await mockSignUp(data);

            showToast('تم إنشاء الحساب بنجاح!', 'يمكنك الآن البدء باستخدام معونة من المزرعة إلى المائدة.', 'bg-success');

            // Close modal on success
            const modal = bootstrap.Modal.getInstance(authModalEl);
            if (modal) modal.hide();

        } else {
            await mockSignIn(data.email, data.password);

            showToast('مرحباً بك مجدداً!', 'تم تسجيل الدخول إلى حسابك بنجاح.', 'bg-success');

            // Close modal on success
            const modal = bootstrap.Modal.getInstance(authModalEl);
            if (modal) modal.hide();
        }
    } catch (error) {
        showToast('خطأ في المصادقة', error.message, 'bg-danger');
        console.error("Auth Error:", error);
    } finally {
        isLoading = false;
        updateUI();
    }
}

// --- Event Listeners and Initialization ---

document.addEventListener('DOMContentLoaded', () => {
    // Event listener to set initial mode when modal is shown
    authModalEl.addEventListener('show.bs.modal', function (event) {
        // Button that triggered the modal
        const button = event.relatedTarget;
        // Extract info from data-auth-mode attributes
        const authMode = button.getAttribute('data-auth-mode') || 'signIn';
        setInitialAuthMode(authMode);
    });

    // Initialize event listeners after DOM is loaded
    updateUI(); // Initial setup
    authForm.addEventListener('submit', handleSubmit);
    roleSelect.addEventListener('change', updateConditionalFields);

    // Hide the modal on successful sign in/up and reset form validation on hide
    authModalEl.addEventListener('hidden.bs.modal', () => {
        authForm.classList.remove('was-validated');
    });
});
