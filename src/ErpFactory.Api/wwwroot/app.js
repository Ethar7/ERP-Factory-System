
// // رابط السيرفر
// const API_BASE_URL = "http://localhost:5101"; 

// const state = {
//   token: localStorage.getItem("erp.token"),
//   user: JSON.parse(localStorage.getItem("erp.user") || "null"),
//   active: "dashboard",
//   cache: {}
// };

// const modules = {
//   dashboard: { title: "الرئيسية" },
//   customers: { title: "العملاء", endpoint: "/api/Customers", columns: [["customerName", "العميل"], ["contactPerson", "مسؤول التواصل"], ["phone", "الهاتف"], ["email", "البريد"], ["address", "العنوان"]], form: [["customerName", "اسم العميل", "text", true], ["contactPerson", "مسؤول التواصل"], ["phone", "الهاتف"], ["email", "البريد", "email"], ["address", "العنوان", "text", false, "wide"]] },
//   projects: { title: "المشاريع", endpoint: "/api/Projects", columns: [["projectName", "المشروع"], ["customer.customerName", "العميل"], ["projectStatus", "الحالة", "status"], ["totalEstimatedBudget", "الميزانية", "money"], ["startDate", "تاريخ البدء", "date"]], form: [["projectName", "اسم المشروع", "text", true, "wide"], ["customerId", "رقم العميل", "number", true], ["totalEstimatedBudget", "الميزانية", "number", true], ["startDate", "تاريخ البدء", "date"]] },
//   inventory: { title: "المخزون", endpoint: "/api/Inventory/items", columns: [["itemName", "الصنف"], ["itemType", "النوع"], ["unit", "الوحدة"], ["currentStock", "الرصيد", "number"], ["averageCost", "متوسط التكلفة", "money"]], form: [["itemName", "اسم الصنف", "text", true], ["itemType", "النوع", "text", true], ["unit", "الوحدة", "text", true], ["currentStock", "الرصيد الحالي", "number", true], ["averageCost", "متوسط التكلفة", "number", true]] },
//   production: { title: "أوامر الإنتاج", endpoint: "/api/ProductionOrders", columns: [["batchNumber", "رقم الدفعة"], ["projectId", "المشروع"], ["targetQuantity", "المستهدف", "number"], ["producedQuantity", "المنتج", "number"], ["productionStatus", "الحالة", "status"], ["orderDate", "التاريخ", "date"]], form: [["projectId", "رقم المشروع", "number", true], ["projectItemId", "بند المشروع", "number", true], ["mixDesignId", "الخلطة", "number", true], ["moldId", "القالب", "number", true], ["batchNumber", "رقم الدفعة"], ["targetQuantity", "الكمية المستهدفة", "number", true], ["laborCost", "تكلفة العمالة", "number"], ["moldDepreciationCost", "إهلاك القالب", "number"]] },
//   delivery: { title: "أوامر التسليم", endpoint: "/api/DeliveryOrders", columns: [["deliveryOrderId", "رقم الأمر"], ["projectId", "المشروع"], ["driverName", "السائق"], ["vehicleNumber", "السيارة"], ["deliveryStatus", "الحالة", "status"], ["deliveryDate", "التاريخ", "date"]], form: [["projectId", "رقم المشروع", "number", true], ["driverName", "اسم السائق"], ["vehicleNumber", "رقم السيارة"], ["loadingTicketNumber", "تذكرة التحميل"], ["deliveryTicketNumber", "تذكرة التسليم"]] },
//   site: { title: "عمليات الموقع", endpoint: "/api/SiteOperations", columns: [["siteOperationId", "رقم العملية"], ["projectId", "المشروع"], ["projectItemId", "البند"], ["installedQuantity", "الكمية المركبة", "number"], ["supervisorLaborCost", "إشراف", "money"], ["dailyExpenses", "مصروفات", "money"]], form: [["projectId", "رقم المشروع", "number", true], ["projectItemId", "بند المشروع", "number", true], ["installedQuantity", "الكمية المركبة", "number", true], ["supervisorLaborCost", "تكلفة الإشراف", "number"], ["dailyExpenses", "مصروفات يومية", "number"]] },
//   accounting: { title: "الحسابات", endpoint: "/api/Accounting/chart-of-accounts", columns: [["accountCode", "كود الحساب"], ["accountName", "اسم الحساب"], ["accountType", "النوع"]], form: [["accountCode", "كود الحساب", "text", true], ["accountName", "اسم الحساب", "text", true], ["accountType", "نوع الحساب", "text", true]] },
//   reports: { title: "التقارير", endpoint: "/api/Reports/project-cost-summary", columns: [["projectName", "المشروع"], ["totalEstimatedBudget", "الميزانية", "money"], ["productionDirectCost", "تكلفة الإنتاج", "money"], ["siteDirectCost", "تكلفة الموقع", "money"], ["totalDirectCost", "إجمالي التكلفة", "money"]] },
//   adminUsers: { title: "إدارة المستخدمين", endpoint: "/api/admin/users" }
// };

// const navLabels = [
//   ["dashboard", "الرئيسية"], ["customers", "العملاء"], ["projects", "المشاريع"], ["inventory", "المخزون"], 
//   ["production", "الإنتاج"], ["delivery", "التسليم"], ["site", "الموقع"], ["accounting", "الحسابات"], 
//   ["reports", "التقارير"], ["adminUsers", "إدارة المستخدمين"]
// ];

// const $ = (selector) => document.querySelector(selector);

// function init() {
//   bindAuthTabs();
//   $("#login-form").addEventListener("submit", onLogin);
//   $("#register-form").addEventListener("submit", onRegister);
//   $("#logout-button").addEventListener("click", logout);
//   $("#refresh-button").addEventListener("click", () => renderCurrent(true));
//   renderNav();
//   state.token ? showApp() : showAuth();
// }

// function bindAuthTabs() {
//   document.querySelectorAll("[data-auth-tab]").forEach((button) => {
//     button.addEventListener("click", () => {
//       document.querySelectorAll("[data-auth-tab]").forEach((x) => x.classList.remove("active"));
//       button.classList.add("active");
//       const isLogin = button.dataset.authTab === "login";
//       $("#login-form").classList.toggle("hidden", !isLogin);
//       $("#register-form").classList.toggle("hidden", isLogin);
//       $("#auth-message").textContent = "";
//     });
//   });
// }

// function renderNav() {
//   const role = state.user?.role;
//   $("#nav-list").innerHTML = navLabels.map(([key, label]) => {
//     if (key === "adminUsers" && role !== "Admin") return "";
//     return `<button class="nav-item" type="button" data-module="${key}">${label}</button>`;
//   }).join("");

//   document.querySelectorAll("[data-module]").forEach((button) => {
//     button.addEventListener("click", () => {
//       state.active = button.dataset.module;
//       renderCurrent();
//     });
//   });
// }

// async function onLogin(event) {
//   event.preventDefault();
//   await authSubmit("/api/Auth/login", new FormData(event.currentTarget), true);
// }

// async function onRegister(event) {
//   event.preventDefault();
//   await authSubmit("/api/Auth/register", new FormData(event.currentTarget), false);
// }

// async function authSubmit(url, formData, isLogin) {
//   $("#auth-message").textContent = "جاري التحميل...";
//   const payload = Object.fromEntries(formData.entries());
//   try {
//     const result = await request(url, { method: "POST", body: payload, skipAuth: true });
//     if (isLogin) {
//       saveSession(result.accessToken, result.user);
//       showApp();
//       return;
//     }
//     $("#auth-message").textContent = "تم إنشاء الحساب. يمكنك تسجيل الدخول الآن.";
//     $("#register-form").reset();
//   } catch (error) {
//     $("#auth-message").textContent = error.message;
//   }
// }

// function saveSession(token, user) {
//   state.token = token;
//   state.user = user;
//   localStorage.setItem("erp.token", token);
//   localStorage.setItem("erp.user", JSON.stringify(user));
// }

// function logout() {
//   localStorage.removeItem("erp.token");
//   localStorage.removeItem("erp.user");
//   state.token = null;
//   state.user = null;
//   state.cache = {};
//   showAuth();
// }

// function showAuth() {
//   $("#auth-view").classList.remove("hidden");
//   $("#app-view").classList.add("hidden");
// }

// function showApp() {
//   $("#auth-view").classList.add("hidden");
//   $("#app-view").classList.remove("hidden");
//   const name = state.user?.fullName || state.user?.username || "مستخدم";
//   const role = state.user?.role ? ` - ${state.user.role}` : "";
//   $("#user-chip").textContent = `${name}${role}`;
//   renderNav(); // تحديث الناف بار بعد تسجيل الدخول
//   renderCurrent();
// }

// function renderCurrent(force = false) {
//   document.querySelectorAll("[data-module]").forEach((button) => {
//     button.classList.toggle("active", button.dataset.module === state.active);
//   });
//   $("#page-title").textContent = modules[state.active].title;
//   if (state.active === "dashboard") return renderDashboard(force);
//   return renderModule(state.active, force);
// }

// async function renderDashboard(force = false) {
//   const content = $("#content");
//   content.innerHTML = loadingMarkup();
//   const endpoints = {
//     customers: "/api/Customers", projects: "/api/Projects", inventory: "/api/Inventory/items",
//     lowStock: "/api/Inventory/low-stock?threshold=5", production: "/api/ProductionOrders", reports: "/api/Reports/project-cost-summary"
//   };
//   const data = {};
//   await Promise.all(Object.entries(endpoints).map(async ([key, url]) => { data[key] = await safeLoad(key, url, force); }));
  
//   const projects = data.projects.rows;
//   const inventory = data.inventory.rows;
//   const production = data.production.rows;
//   const reports = data.reports.rows;
//   const activeProjects = projects.filter((x) => (x.projectStatus || "").toLowerCase() !== "completed").length;
//   const inventoryValue = inventory.reduce((sum, x) => sum + numberOf(x.currentStock) * numberOf(x.averageCost), 0);
//   const produced = production.reduce((sum, x) => sum + numberOf(x.producedQuantity), 0);
//   const directCost = reports.reduce((sum, x) => sum + numberOf(x.totalDirectCost), 0);

//   content.innerHTML = `
//     <div class="stats-grid">
//       ${stat("العملاء", data.customers.rows.length)} ${stat("مشاريع نشطة", activeProjects)}
//       ${stat("قيمة المخزون", formatMoney(inventoryValue))} ${stat("إجمالي المنتج", formatNumber(produced))}
//     </div>
//     <div class="stats-grid">
//       ${stat("أصناف منخفضة", data.lowStock.rows.length)} ${stat("أوامر إنتاج", production.length)}
//       ${stat("تكلفة مباشرة", formatMoney(directCost))} ${stat("صلاحيات مقيدة", countErrors(data))}
//     </div>
//     ${permissionNotes(data)}
//     ${tablePanel("آخر المشاريع", projects.slice(0, 6), modules.projects.columns)}
//     ${tablePanel("أرصدة مخزون مهمة", inventory.slice(0, 6), modules.inventory.columns)}
//   `;
// }

// async function renderModule(key, force = false) {
//   const config = modules[key];
//   const content = $("#content");
//   content.innerHTML = loadingMarkup();
//   const result = await safeLoad(key, config.endpoint, force);

// if (key === "adminUsers") {
//   content.innerHTML = `
//     <section class="panel"><h3>إدارة صلاحيات المستخدمين</h3>
//       <table><thead><tr><th>المستخدم</th><th>الصلاحية (Role)</th></tr></thead>
//       <tbody>${result.rows.map(user => `<tr><td>${user.username}</td><td>
//         <select onchange="changeUserRole(${user.userId}, this.value)">
//           <option value="Admin" ${user.role === 'Admin' ? 'selected' : ''}>مدير النظام (Admin)</option>
//           <option value="ProjectManager" ${user.role === 'ProjectManager' ? 'selected' : ''}>مدير مشاريع</option>
//           <option value="InventoryUser" ${user.role === 'InventoryUser' ? 'selected' : ''}>مسؤول مخزون</option>
//           <option value="Accountant" ${user.role === 'Accountant' ? 'selected' : ''}>محاسب</option>
//         </select></td></tr>`).join("")}</tbody></table></section>`;

//   } else {
//     content.innerHTML = `${config.form ? formPanel(key, config.form) : ""}${result.error ? errorPanel(result.error) : ""}${tablePanel(config.title, result.rows, config.columns)}`;
//     const form = $(`#${key}-form`);
//     if (form) form.addEventListener("submit", (event) => submitCreate(event, key));
//   }
// }

// async function changeUserRole(userId, newRole) {
//   try {
//     await request(`/api/admin/users/${userId}/role`, { method: "PUT", body: newRole });
//     toast("تم تحديث الصلاحية");
//     renderModule("adminUsers", true);
//   } catch (error) { toast(error.message, true); }
// }

// async function submitCreate(event, key) {
//   event.preventDefault();
//   const config = modules[key];
//   const formData = new FormData(event.currentTarget);
//   const body = Object.fromEntries(formData.entries());
//   for (const [field, , type] of config.form) {
//     if (body[field] === "") body[field] = null;
//     else if (type === "number" && body[field] !== null) body[field] = Number(body[field]);
//   }
//   try {
//     await request(config.endpoint, { method: "POST", body });
//     event.currentTarget.reset();
//     delete state.cache[key];
//     toast("تم الحفظ بنجاح");
//     renderModule(key, true);
//   } catch (error) { toast(error.message, true); }
// }

// async function safeLoad(key, url, force = false) {
//   if (!force && state.cache[key]) return state.cache[key];
//   try {
//     const rows = await request(url);
//     const result = { rows: Array.isArray(rows) ? rows : [], error: null };
//     state.cache[key] = result;
//     return result;
//   } catch (error) {
//     const result = { rows: [], error: error.message };
//     state.cache[key] = result;
//     return result;
//   }
// }

// async function request(url, options = {}) {
//   const headers = { "Content-Type": "application/json" };
//   if (!options.skipAuth && state.token) headers.Authorization = `Bearer ${state.token}`;
//   const fullUrl = url.startsWith("http") ? url : `${API_BASE_URL}${url}`;
//   const response = await fetch(fullUrl, {
//     method: options.method || "GET",
//     headers,
//     body: options.body ? JSON.stringify(options.body) : undefined
//   });
//   if (response.status === 401) { logout(); throw new Error("انتهت الجلسة."); }
//   const text = await response.text();
//   let json = null;
//   try { json = text ? JSON.parse(text) : null; } catch { json = null; }
//   if (!response.ok || json?.success === false) {
//     let message = json?.message || statusMessage(response.status);
//     throw new Error(message);
//   }
//   return json?.data ?? json;
// }

// function formPanel(key, fields) {
//   return `<section class="form-panel"><form id="${key}-form" class="form-grid">${fields.map(([name, label, type = "text", required = false, span = ""]) => `<label class="${span}">${label} ${required ? '<span style="color:red">*</span>' : ''}<input name="${name}" type="${type}" ${required ? "required" : ""} ${type === "number" ? 'step="any"' : ""}></label>`).join("")}<button class="primary" type="submit">إضافة</button></form></section>`;
// }

// function tablePanel(title, rows, columns) {
//   return `<section class="panel"><div class="panel-header"><h3>${title}</h3><span class="status-pill">${rows.length} سجل</span></div>${rows.length ? `<div class="table-wrap"><table><thead><tr>${columns.map(([, label]) => `<th>${label}</th>`).join("")}</tr></thead><tbody>${rows.map((row) => `<tr>${columns.map(([field, , type]) => `<td>${formatCell(valueAt(row, field), type)}</td>`).join("")}</tr>`).join("")}</tbody></table></div>` : `<div class="empty-state">لا توجد بيانات.</div>`}</section>`;
// }

// function stat(label, value) { return `<article class="stat"><span>${label}</span><strong>${value}</strong></article>`; }
// function errorPanel(message) { return `<section class="toast error">${message}</section>`; }
// function permissionNotes(data) { const errors = Object.values(data).map((x) => x.error).filter(Boolean); return errors.length ? `<section class="toast error">تنبيه: ${errors.join(" | ")}</section>` : ""; }
// function countErrors(data) { return Object.values(data).filter((x) => x.error).length; }
// function valueAt(row, path) { return path.split(".").reduce((value, key) => value?.[key], row); }
// function formatCell(value, type) {
//   if (value === null || value === undefined || value === "") return "-";
//   if (type === "date") return new Date(value).toLocaleDateString("ar-EG");
//   if (type === "money") return formatMoney(value);
//   if (type === "number") return formatNumber(value);
//   if (type === "status") return `<span class="status-pill">${value}</span>`;
//   return value;
// }
// function formatMoney(value) { return new Intl.NumberFormat("ar-EG", { style: "currency", currency: "EGP", maximumFractionDigits: 0 }).format(Number(value) || 0); }
// function formatNumber(value) { return new Intl.NumberFormat("ar-EG", { maximumFractionDigits: 2 }).format(Number(value) || 0); }
// function numberOf(value) { const n = Number(value); return Number.isFinite(n) ? n : 0; }
// function loadingMarkup() { return `<section class="panel"><div class="empty-state">جاري التحميل...</div></section>`; }
// function statusMessage(status) { return status === 403 ? "ليس لديك صلاحية." : "حدث خطأ."; }
// function toast(message, isError = false) {
//   const element = $("#toast");
//   if (!element) return;
//   element.textContent = message;
//   element.classList.toggle("error", isError);
//   element.classList.remove("hidden");
//   window.clearTimeout(toast.timer);
//   toast.timer = window.setTimeout(() => element.classList.add("hidden"), 4500);
// }

// init();

// رابط السيرفر
const API_BASE_URL = "https://etharosama-001-site1.ftempurl.com"; 

const state = {
  token: localStorage.getItem("erp.token"),
  user: JSON.parse(localStorage.getItem("erp.user") || "null"),
  active: "dashboard",
  cache: {}
};

const modules = {
  dashboard: { title: "الرئيسية" },
  customers: { title: "العملاء", endpoint: "/api/Customers", columns: [["customerName", "العميل"], ["contactPerson", "مسؤول التواصل"], ["phone", "الهاتف"], ["email", "البريد"], ["address", "العنوان"]], form: [["customerName", "اسم العميل", "text", true], ["contactPerson", "مسؤول التواصل"], ["phone", "الهاتف"], ["email", "البريد", "email"], ["address", "العنوان", "text", false, "wide"]] },
  projects: { title: "المشاريع", endpoint: "/api/Projects", columns: [["projectName", "المشروع"], ["customer.customerName", "العميل"], ["projectStatus", "الحالة", "status"], ["totalEstimatedBudget", "الميزانية", "money"], ["startDate", "تاريخ البدء", "date"]], form: [["projectName", "اسم المشروع", "text", true, "wide"], ["customerId", "رقم العميل", "number", true], ["totalEstimatedBudget", "الميزانية", "number", true], ["startDate", "تاريخ البدء", "date"]] },
  inventory: { title: "المخزون", endpoint: "/api/Inventory/items", columns: [["itemName", "الصنف"], ["itemType", "النوع"], ["unit", "الوحدة"], ["currentStock", "الرصيد", "number"], ["averageCost", "متوسط التكلفة", "money"]], form: [["itemName", "اسم الصنف", "text", true], ["itemType", "النوع", "text", true], ["unit", "الوحدة", "text", true], ["currentStock", "الرصيد الحالي", "number", true], ["averageCost", "متوسط التكلفة", "number", true]] },
  production: { title: "أوامر الإنتاج", endpoint: "/api/ProductionOrders", columns: [["batchNumber", "رقم الدفعة"], ["projectId", "المشروع"], ["targetQuantity", "المستهدف", "number"], ["producedQuantity", "المنتج", "number"], ["productionStatus", "الحالة", "status"], ["orderDate", "التاريخ", "date"]], form: [["projectId", "رقم المشروع", "number", true], ["projectItemId", "بند المشروع", "number", true], ["mixDesignId", "الخلطة", "number", true], ["moldId", "القالب", "number", true], ["batchNumber", "رقم الدفعة"], ["targetQuantity", "الكمية المستهدفة", "number", true], ["laborCost", "تكلفة العمالة", "number"], ["moldDepreciationCost", "إهلاك القالب", "number"]] },
  delivery: { title: "أوامر التسليم", endpoint: "/api/DeliveryOrders", columns: [["deliveryOrderId", "رقم الأمر"], ["projectId", "المشروع"], ["driverName", "السائق"], ["vehicleNumber", "السيارة"], ["deliveryStatus", "الحالة", "status"], ["deliveryDate", "التاريخ", "date"]], form: [["projectId", "رقم المشروع", "number", true], ["driverName", "اسم السائق"], ["vehicleNumber", "رقم السيارة"], ["loadingTicketNumber", "تذكرة التحميل"], ["deliveryTicketNumber", "تذكرة التسليم"]] },
  site: { title: "عمليات الموقع", endpoint: "/api/SiteOperations", columns: [["siteOperationId", "رقم العملية"], ["projectId", "المشروع"], ["projectItemId", "البند"], ["installedQuantity", "الكمية المركبة", "number"], ["supervisorLaborCost", "إشراف", "money"], ["dailyExpenses", "مصروفات", "money"]], form: [["projectId", "رقم المشروع", "number", true], ["projectItemId", "بند المشروع", "number", true], ["installedQuantity", "الكمية المركبة", "number", true], ["supervisorLaborCost", "تكلفة الإشراف", "number"], ["dailyExpenses", "مصروفات يومية", "number"]] },
  accounting: { title: "الحسابات", endpoint: "/api/Accounting/chart-of-accounts", columns: [["accountCode", "كود الحساب"], ["accountName", "اسم الحساب"], ["accountType", "النوع"]], form: [["accountCode", "كود الحساب", "text", true], ["accountName", "اسم الحساب", "text", true], ["accountType", "نوع الحساب", "text", true]] },
  reports: { title: "التقارير", endpoint: "/api/Reports/project-cost-summary", columns: [["projectName", "المشروع"], ["totalEstimatedBudget", "الميزانية", "money"], ["productionDirectCost", "تكلفة الإنتاج", "money"], ["siteDirectCost", "تكلفة الموقع", "money"], ["totalDirectCost", "إجمالي التكلفة", "money"]] },
  adminUsers: { title: "إدارة المستخدمين", endpoint: "/api/admin/users" }
};

const navLabels = [
  ["dashboard", "الرئيسية"], ["customers", "العملاء"], ["projects", "المشاريع"], ["inventory", "المخزون"], 
  ["production", "الإنتاج"], ["delivery", "التسليم"], ["site", "الموقع"], ["accounting", "الحسابات"], 
  ["reports", "التقارير"], ["adminUsers", "إدارة المستخدمين"]
];

const $ = (selector) => document.querySelector(selector);

function init() {
  bindAuthTabs();
  $("#login-form").addEventListener("submit", onLogin);
  $("#register-form").addEventListener("submit", onRegister);
  $("#logout-button").addEventListener("click", logout);
  $("#refresh-button").addEventListener("click", () => renderCurrent(true));
  state.token ? showApp() : showAuth();
}

// تعديل: دالة بناء القائمة للتأكد من الصلاحيات
function renderNav() {
  const role = state.user?.role || "";
  console.log("Current User Role:", role); // للتصحيح في الكونسول
  
  const navList = $("#nav-list");
  navList.innerHTML = navLabels.map(([key, label]) => {
    // إخفاء إدارة المستخدمين إذا لم يكن الدور Admin
    if (key === "adminUsers" && role !== "Admin") return "";
    return `<button class="nav-item" type="button" data-module="${key}">${label}</button>`;
  }).join("");

  navList.querySelectorAll("[data-module]").forEach((button) => {
    button.addEventListener("click", () => {
      state.active = button.dataset.module;
      renderCurrent();
    });
  });
}

function showApp() {
  $("#auth-view").classList.add("hidden");
  $("#app-view").classList.remove("hidden");
  const name = state.user?.fullName || state.user?.username || "مستخدم";
  const role = state.user?.role ? ` - ${state.user.role}` : "";
  $("#user-chip").textContent = `${name}${role}`;
  
  // استدعاء renderNav هنا يضمن وجود بيانات المستخدم
  renderNav(); 
  renderCurrent();
}

// (باقي الدوال تبقى كما هي في كودك - تأكدي من لصقها هنا)
async function onLogin(event) { event.preventDefault(); await authSubmit("/api/Auth/login", new FormData(event.currentTarget), true); }
async function onRegister(event) { event.preventDefault(); await authSubmit("/api/Auth/register", new FormData(event.currentTarget), false); }
async function authSubmit(url, formData, isLogin) {
  $("#auth-message").textContent = "جاري التحميل...";
  const payload = Object.fromEntries(formData.entries());
  try {
    const result = await request(url, { method: "POST", body: payload, skipAuth: true });
    if (isLogin) { saveSession(result.accessToken, result.user); showApp(); return; }
    $("#auth-message").textContent = "تم إنشاء الحساب."; $("#register-form").reset();
  } catch (error) { $("#auth-message").textContent = error.message; }
}

function saveSession(token, user) { state.token = token; state.user = user; localStorage.setItem("erp.token", token); localStorage.setItem("erp.user", JSON.stringify(user)); }
function logout() { localStorage.clear(); state.token = null; state.user = null; state.cache = {}; showAuth(); }
function showAuth() { $("#auth-view").classList.remove("hidden"); $("#app-view").classList.add("hidden"); }
function renderCurrent(force = false) {
  document.querySelectorAll("[data-module]").forEach((b) => b.classList.toggle("active", b.dataset.module === state.active));
  $("#page-title").textContent = modules[state.active].title;
  state.active === "dashboard" ? renderDashboard(force) : renderModule(state.active, force);
}
async function renderModule(key, force = false) {
  const config = modules[key];
  const content = $("#content");
  content.innerHTML = loadingMarkup();
  const result = await safeLoad(key, config.endpoint, force);
  if (key === "adminUsers") {
    content.innerHTML = `<section class="panel"><h3>إدارة صلاحيات المستخدمين</h3><table><thead><tr><th>المستخدم</th><th>الصلاحية</th></tr></thead><tbody>${result.rows.map(u => `<tr><td>${u.username}</td><td><select onchange="changeUserRole(${u.userId}, this.value)"><option value="Admin" ${u.role === 'Admin' ? 'selected' : ''}>Admin</option><option value="ProjectManager" ${u.role === 'ProjectManager' ? 'selected' : ''}>ProjectManager</option><option value="InventoryUser" ${u.role === 'InventoryUser' ? 'selected' : ''}>InventoryUser</option><option value="Accountant" ${u.role === 'Accountant' ? 'selected' : ''}>Accountant</option></select></td></tr>`).join("")}</tbody></table></section>`;
  } else {
    content.innerHTML = `${config.form ? formPanel(key, config.form) : ""}${result.error ? errorPanel(result.error) : ""}${tablePanel(config.title, result.rows, config.columns)}`;
    const f = $(`#${key}-form`); if(f) f.addEventListener("submit", (e) => submitCreate(e, key));
  }
}
async function changeUserRole(userId, newRole) { try { await request(`/api/admin/users/${userId}/role`, { method: "PUT", body: JSON.stringify(newRole) }); toast("تم التحديث"); renderModule("adminUsers", true); } catch (e) { toast(e.message, true); } }
async function request(url, options = {}) {
  const headers = { "Content-Type": "application/json" };
  if (!options.skipAuth && state.token) headers.Authorization = `Bearer ${state.token}`;
  
  const fullUrl = url.startsWith("http") ? url : `${API_BASE_URL}${url}`;
  
  const response = await fetch(fullUrl, {
    method: options.method || "GET",
    headers,
    body: options.body ? JSON.stringify(options.body) : undefined
  });

  const text = await response.text(); // قراءة الرد كنص أولاً
  let json;
  try {
    json = JSON.parse(text);
  } catch (e) {
    // إذا لم يكن الرد JSON، فهذا يعني أن السيرفر أرسل خطأ HTML أو نص عادي
    if (!response.ok) throw new Error(`خطأ سيرفر (${response.status}): ${text.substring(0, 100)}`);
  }

  if (!response.ok) {
    // عرض رسالة الخطأ القادمة من السيرفر
    throw new Error(json?.message || json?.title || "فشل الاتصال بالسيرفر");
  }

  return json?.data ?? json;
}
function bindAuthTabs() { document.querySelectorAll("[data-auth-tab]").forEach(b => b.addEventListener("click", () => { document.querySelectorAll("[data-auth-tab]").forEach(x => x.classList.remove("active")); b.classList.add("active"); const isLogin = b.dataset.authTab === "login"; $("#login-form").classList.toggle("hidden", !isLogin); $("#register-form").classList.toggle("hidden", isLogin); })); }
function loadingMarkup() { return `<section class="panel">جاري التحميل...</section>`; }
function tablePanel(t, r, c) { return `<section class="panel"><h3>${t}</h3><table><thead><tr>${c.map(col => `<th>${col[1]}</th>`).join("")}</tr></thead><tbody>${r.map(row => `<tr>${c.map(col => `<td>${row[col[0]] || '-'}</td>`).join("")}</tr>`).join("")}</tbody></table></section>`; }
function toast(m, e=false) { const el = $("#toast"); el.textContent = m; el.classList.remove("hidden"); setTimeout(() => el.classList.add("hidden"), 3000); }

init();