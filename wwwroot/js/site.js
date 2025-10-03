// Danh sách sản phẩm giả lập
const products = [
    { id: 1, name: "Tôm hùm Alaska", category: "Hải Sản", img: "https://c.pxhere.com/photos/98/a1/leaf_green_line_vein-1286313.jpg!d", price: 500000 },
    { id: 2, name: "Gia vị lẩu Thái", category: "Gia Vị", img: "https://c.pxhere.com/photos/98/a1/leaf_green_line_vein-1286313.jpg!d", price: 50000 },
    { id: 3, name: "Son môi đỏ", category: "Mỹ Phẩm", img: "https://c.pxhere.com/photos/98/a1/leaf_green_line_vein-1286313.jpg!d", price: 200000 },
    { id: 4, name: "Áo thun Unisex", category: "Quần Áo", img: "https://c.pxhere.com/photos/98/a1/leaf_green_line_vein-1286313.jpg!d", price: 120000 },
    { id: 5, name: "Mì cay Hàn Quốc", category: "Gia Vị", img: "https://c.pxhere.com/photos/98/a1/leaf_green_line_vein-1286313.jpg!d", price: 40000 },
    { id: 6, name: "Cua Hoàng Đế", category: "Hải Sản", img: "https://c.pxhere.com/photos/98/a1/leaf_green_line_vein-1286313.jpg!d", price: 800000 },
    { id: 7, name: "Sashimi Cá Hồi", category: "Hải Sản", img: "https://c.pxhere.com/photos/98/a1/leaf_green_line_vein-1286313.jpg!d", price: 300000 },
    { id: 8, name: "Muối Ớt Tây Ninh", category: "Gia Vị", img: "https://c.pxhere.com/photos/98/a1/leaf_green_line_vein-1286313.jpg!d", price: 25000 },
];

let productList = document.getElementById("productList");
let cartItems = document.getElementById("cartItems");
let total = document.getElementById("total");
let cart = [];

/* ---------- Hiển thị sản phẩm bằng Bootstrap Card ---------- */
function renderProducts(list) {
    productList.innerHTML = "";
    list.forEach(p => {
        productList.innerHTML += `
        <div class="col-sm-6 col-md-4 col-lg-3">
            <div class="card h-100 shadow-sm product">
                <img src="${p.img}" class="card-img-top" alt="${p.name}">
                <div class="card-body text-center">
                    <h5 class="card-title">${p.name}</h5>
                    <p class="card-text text-muted">${p.price.toLocaleString()}đ</p>
                    <button class="btn btn-warning w-100" onclick="addToCart(${p.id})">
                        <i class="bi bi-cart-plus"></i> Thêm vào giỏ
                    </button>
                </div>
            </div>
        </div>`;
    });
}
renderProducts(products);

/* ---------- Thêm sản phẩm vào giỏ ---------- */
function addToCart(id) {
    let product = products.find(p => p.id === id);
    let item = cart.find(c => c.id === id);
    if (item) {
        item.quantity++;
    } else {
        cart.push({ ...product, quantity: 1 });
    }
    updateCart();

    // Mở offcanvas giỏ hàng
    let cartOffcanvas = new bootstrap.Offcanvas(document.getElementById('cartOffcanvas'));
    cartOffcanvas.show();
}

/* ---------- Cập nhật giỏ hàng ---------- */
function updateCart() {
    cartItems.innerHTML = "";
    let sum = 0;
    cart.forEach((item, i) => {
        let lineTotal = item.price * item.quantity;
        sum += lineTotal;
        cartItems.innerHTML += `
            <li class="list-group-item d-flex justify-content-between align-items-center">
                ${item.name} x${item.quantity}
                <div>
                    <span class="me-2">${lineTotal.toLocaleString()}đ</span>
                    <button class="btn btn-sm btn-outline-danger" onclick="removeItem(${i})">
                        <i class="bi bi-trash"></i>
                    </button>
                </div>
            </li>`;
    });
    total.innerText = "Tổng: " + sum.toLocaleString() + "đ";
}

/* ---------- Xóa sản phẩm trong giỏ ---------- */
function removeItem(index) {
    cart.splice(index, 1);
    updateCart();
}

/* ---------- Tìm kiếm sản phẩm ---------- */
function searchProduct() {
    let keyword = document.getElementById("searchInput").value.toLowerCase();
    let filtered = products.filter(p => p.name.toLowerCase().includes(keyword));
    renderProducts(filtered);
}

/* ---------- Lọc theo danh mục ---------- */
function filterCategory(cat) {
    if (cat === "all") {
        renderProducts(products);
    } else {
        let filtered = products.filter(p => p.category === cat);
        renderProducts(filtered);
    }
}

/* ---------- Sắp xếp sản phẩm ---------- */
function sortProducts(type) {
    let sorted = [...products];
    if (type === "asc") sorted.sort((a, b) => a.price - b.price);
    if (type === "desc") sorted.sort((a, b) => b.price - a.price);
    renderProducts(sorted);
}

/* ---------- Thanh toán ---------- */
function checkout() {
    if (cart.length === 0) {
        alert("Giỏ hàng trống!");
    } else {
        alert("Thanh toán thành công " + cart.length + " sản phẩm!");
        cart = [];
        updateCart();

        // Đóng offcanvas sau khi checkout
        let cartOffcanvas = bootstrap.Offcanvas.getInstance(document.getElementById('cartOffcanvas'));
        cartOffcanvas.hide();
    }
}

/* ---------- LOGIN Modal ---------- */
function doLogin(e) {
    e.preventDefault();
    let user = document.getElementById("username").value;
    alert("Xin chào " + user + "!");

    // Đóng modal sau khi login
    let loginModal = bootstrap.Modal.getInstance(document.getElementById('loginModal'));
    loginModal.hide();
}
const music = document.getElementById("background-music");
const toggleBtn = document.getElementById("toggleMusic");

toggleBtn.addEventListener("click", () => {
    if (music.paused) {
        music.play();
        toggleBtn.textContent = "🔇 Mute";
    } else {
        music.pause();
        toggleBtn.textContent = "🔊 Chiller !!";
    }
});
