import React, { useEffect, useRef, useState } from 'react';
import { createRoot } from 'react-dom/client';
import { BrowserRouter, Routes, Route, Link, useNavigate, useParams, useSearchParams } from 'react-router-dom';
import { Search, UserRound, ShoppingCart, Grid2X2, List, ChevronRight, Menu, MapPin, Package, LogOut, Mail, Phone } from 'lucide-react';
import axios from 'axios';
import './src.css';

const api = axios.create({ baseURL: import.meta.env.VITE_API_URL || 'http://localhost:5000' });
api.interceptors.request.use(config => {
  const token = localStorage.getItem('token');
  if (token) config.headers.Authorization = 'Bearer ' + token;
  return config;
});
const money = value => Number(value || 0).toLocaleString('vi-VN') + 'đ';
const cartKey = item => (item.kind || 'product') + ':' + item.id;
function flyToCart(source) {
  if (!source || window.matchMedia('(prefers-reduced-motion: reduce)').matches) return;
  const target = document.querySelector('[data-cart-target]');
  const image = source.closest('.product-card, .detail-layout')?.querySelector('img');
  if (!target || !image || !image.complete || !image.naturalWidth) return;
  const from = image.getBoundingClientRect();
  const to = target.getBoundingClientRect();
  const size = Math.min(86, from.width, from.height);
  const flyer = image.cloneNode();
  Object.assign(flyer.style, { position: 'fixed', left: `${from.left + (from.width - size) / 2}px`, top: `${from.top + (from.height - size) / 2}px`, width: `${size}px`, height: `${size}px`, objectFit: 'cover', borderRadius: '14px', pointerEvents: 'none', zIndex: '1000', boxShadow: '0 8px 24px #0003' });
  document.body.appendChild(flyer);
  const dx = to.left + to.width / 2 - (from.left + from.width / 2);
  const dy = to.top + to.height / 2 - (from.top + from.height / 2);
  const animation = flyer.animate([
    { transform: 'translate(0, 0) scale(1)', borderRadius: '14px', opacity: 1 },
    { transform: `translate(${dx * .35}px, ${Math.min(-70, dy * .35)}px) scale(.72)`, borderRadius: '50%', opacity: 1, offset: .42 },
    { transform: `translate(${dx}px, ${dy}px) scale(.16)`, borderRadius: '50%', opacity: .2 }
  ], { duration: 650, easing: 'cubic-bezier(.2,.7,.25,1)' });
  animation.finished.then(() => flyer.remove(), () => flyer.remove());
  target.animate([{ transform: 'scale(1)' }, { transform: 'scale(1.25)' }, { transform: 'scale(1)' }], { duration: 300, delay: 520 });
}
const Box = ({ children }) => <div className="site-container">{children}</div>;
const Button = ({ children, className = '', ...props }) => <button {...props} className={'action-button ' + className}>{children}</button>;
const Field = ({ label, ...props }) => <label className="block mb-3"><span className="block font-semibold mb-1">{label}</span><input {...props} className="w-full border border-orange/20 bg-white rounded-xl px-3 py-2" /></label>;

const LOAD_FAILED = Symbol('load failed');
function useFetch(url, onError, errorValue = null) {
  const [data, setData] = useState(null);
  useEffect(() => {
    let active = true;
    setData(null);
    api.get(url).then(response => { if (active) setData(response.data); })
      .catch(error => { if (active) { setData(errorValue); onError(error.response?.data?.title || 'Không thể tải dữ liệu'); } });
    return () => { active = false; };
  }, [url]);
  return data;
}

function Header({ user, cart, logout, err }) {
  const categories = useFetch('/api/categories', err);
  const [search, setSearch] = useState('');
  const [menuOpen, setMenuOpen] = useState(false);
  const [accountOpen, setAccountOpen] = useState(false);
  const accountRef = useRef(null);
  const navigate = useNavigate();
  const count = cart.reduce((total, item) => total + item.qty, 0);
  const submit = event => { event.preventDefault(); navigate('/products?q=' + encodeURIComponent(search.trim())); setMenuOpen(false); };
  useEffect(() => {
    if (!accountOpen) return;
    const closeOutside = event => { if (!accountRef.current?.contains(event.target)) setAccountOpen(false); };
    const closeEscape = event => { if (event.key === 'Escape') setAccountOpen(false); };
    document.addEventListener('pointerdown', closeOutside);
    document.addEventListener('keydown', closeEscape);
    return () => { document.removeEventListener('pointerdown', closeOutside); document.removeEventListener('keydown', closeEscape); };
  }, [accountOpen]);
  return <header className="site-header">
    <div className="header-main site-container">
      <Link to="/" className="brand" aria-label="Snack Chiwiwi - Trang chủ"><img className="brand-logo" src="/images/snack-chiwiwi-logo.svg" alt="" /><span className="brand-copy"><strong>Snack Chiwiwi</strong><small>Ăn vui mỗi ngày</small></span></Link>
      <form className="header-search" role="search" onSubmit={submit}><input value={search} onChange={event => setSearch(event.target.value)} placeholder="Tìm món ăn vặt bạn thích..." aria-label="Tìm sản phẩm" /><button aria-label="Tìm kiếm"><Search size={22} /></button></form>
      <div className="header-actions">
        <Link to="/cart" className="header-action"><span className="cart-icon" data-cart-target><ShoppingCart size={29} /><b>{count}</b></span><span><strong>Giỏ hàng của bạn</strong><small>{count ? count + ' món đang chọn' : 'Chưa có sản phẩm'}</small></span></Link>
        <div className="account-control" ref={accountRef}>
          {user ? <button type="button" className="header-action account-trigger" aria-haspopup="menu" aria-expanded={accountOpen} onClick={() => setAccountOpen(open => !open)}>
            <UserRound size={26} /><span><strong>Tài khoản</strong><small>{user.name}</small></span>
          </button> : <Link to="/login" className="header-action"><UserRound size={26} /><span><strong>Tài khoản</strong><small>Đăng nhập, đăng ký</small></span></Link>}
          {user && accountOpen && <div className="account-menu" role="menu" aria-label="Tài khoản của bạn">
            <div className="account-menu-profile"><div className="account-avatar">{user.name?.trim()?.charAt(0)?.toUpperCase() || 'K'}</div><div><strong>{user.name}</strong><small>{user.email}</small></div></div>
            <Link role="menuitem" to="/orders" onClick={() => setAccountOpen(false)}><Package size={19} /> Đơn hàng của tôi</Link>
            <Link role="menuitem" to="/addresses" onClick={() => setAccountOpen(false)}><MapPin size={19} /> Sổ địa chỉ giao hàng</Link>
            <button type="button" role="menuitem" onClick={() => { setAccountOpen(false); logout(); }}><LogOut size={19} /> Đăng xuất</button>
          </div>}
        </div>
        <button className="mobile-menu" onClick={() => setMenuOpen(!menuOpen)} aria-label="Mở danh mục"><Menu size={25} /></button>
      </div>
    </div>
    <nav className={'category-nav ' + (menuOpen ? 'open' : '')} aria-label="Danh mục sản phẩm"><div className="site-container category-nav-inner">
      <Link to="/products" onClick={() => setMenuOpen(false)}>Tất cả sản phẩm</Link>
      {categories?.map(category => <Link to={'/products?categoryId=' + category.id} onClick={() => setMenuOpen(false)} key={category.id}>{category.name}</Link>)}
      <Link to="/combos" onClick={() => setMenuOpen(false)}>Combo ăn vặt</Link>
      {user?.role === 'Admin' && <Link to="/admin">Quản trị</Link>}
    </div></nav>
  </header>;
}

function App() {
  const [user, setUser] = useState(JSON.parse(localStorage.getItem('user') || 'null'));
  const [cart, setCart] = useState(JSON.parse(localStorage.getItem('cart') || '[]'));
  const [error, setError] = useState('');
  const navigate = useNavigate();
  const showError = value => setError(typeof value === 'string' ? value : JSON.stringify(value));
  useEffect(() => localStorage.setItem('cart', JSON.stringify(cart)), [cart]);
  const add = (product, source) => {
    flyToCart(source);
    setCart(items => {
    const existing = items.find(item => item.id === product.id && (item.kind || 'product') === (product.kind || 'product'));
    return existing ? items.map(item => item.id === product.id && (item.kind || 'product') === (product.kind || 'product') ? { ...item, qty: Math.min(item.qty + 1, product.stock) } : item) : [...items, { ...product, qty: 1 }];
    });
  };
  const logout = () => { localStorage.removeItem('token'); localStorage.removeItem('user'); setUser(null); navigate('/'); };
  return <div className="app-shell">
    <Header user={user} cart={cart} logout={logout} err={showError} />
    {error && <div className="site-error" role="alert">{error}<button onClick={() => setError('')} aria-label="Đóng thông báo">×</button></div>}
    <div className="app-content"><Routes>
      <Route path="/" element={<Home add={add} err={showError} />} />
      <Route path="/products" element={<Products add={add} err={showError} />} />
      <Route path="/products/:id" element={<Detail add={add} err={showError} />} />
      <Route path="/combos" element={<Combos add={add} err={showError} />} />
      <Route path="/combos/:id" element={<ComboDetail add={add} err={showError} />} />
      <Route path="/cart" element={<Cart cart={cart} setCart={setCart} user={user} err={showError} />} />
      <Route path="/addresses" element={<Addresses err={showError} />} />
      <Route path="/login" element={<Auth user={user} setUser={setUser} err={showError} />} />
      <Route path="/orders" element={<Orders err={showError} />} />
      <Route path="/orders/:id" element={<OrderDetail err={showError} />} />
      <Route path="/admin" element={<Admin err={showError} />} />
      <Route path="/admin/orders/:id" element={<AdminOrderDetail err={showError} />} />
    </Routes></div>
    <footer className="site-footer"><Box><div className="footer-grid">
      <section className="footer-about"><h2>Snack Chiwiwi</h2><p>Góc ăn vặt với bánh kẹo, đồ khô, nước uống và nhiều combo để bạn lựa chọn. Xem sản phẩm, thêm vào giỏ và đặt hàng COD ngay trên website.</p></section>
      <section className="footer-contact"><h2>Thông tin liên hệ</h2><address>
        <p><MapPin size={18} aria-hidden="true" /><span>Nhổn, Bắc Từ Liêm, Hà Nội</span></p>
        <p><Phone size={18} aria-hidden="true" /><a href="tel:0123456789">0123456789</a></p>
        <p><Mail size={18} aria-hidden="true" /><a href="mailto:abc@gamil.com">abc@gamil.com</a></p>
      </address></section>
    </div><div className="footer-bottom"><span>© Snack Chiwiwi</span></div></Box></footer>
  </div>;
}

function Breadcrumb({ current }) {
  return <div className="breadcrumb"><Box><Link to="/">Trang chủ</Link><ChevronRight size={14} /><Link to="/products">Sản phẩm</Link>{current && <><ChevronRight size={14} /><span>{current}</span></>}</Box></div>;
}

function Card({ p, add, view = 'grid' }) {
  return <article className={'product-card ' + (view === 'list' ? 'list-card' : '')}>
    <Link to={'/products/' + p.id} className="product-image"><img src={p.imageUrl} alt={p.name} loading="lazy" onError={event => event.currentTarget.style.visibility = 'hidden'} /></Link>
    <div className="product-body"><span className="product-category">{p.categoryName}</span><Link to={'/products/' + p.id} className="product-name">{p.name}</Link>{view === 'list' && <p className="product-description">{p.description}</p>}<strong className="product-price">{money(p.price)}</strong><Button disabled={p.stock < 1} onClick={event => add(p, event.currentTarget)} className="product-add"><ShoppingCart size={17} />{p.stock ? 'Thêm vào giỏ' : 'Hết hàng'}</Button></div>
  </article>;
}

function Grid({ items, add, view = 'grid' }) {
  return items?.length ? <div className={'product-grid ' + (view === 'list' ? 'list-view' : '')}>{items.map(product => <Card key={product.id} p={product} add={add} view={view} />)}</div> : <div className="empty-products"><Search size={32} /><strong>Không tìm thấy sản phẩm</strong><span>Thử thay đổi từ khóa hoặc bộ lọc.</span></div>;
}

function ComboCard({ combo, add }) {
  const saving = combo.originalPrice - combo.comboPrice;
  return <article className="product-card">
    <Link to={'/combos/' + combo.id} className="product-image"><img src={combo.imageUrl} alt={combo.name} loading="lazy" />{saving > 0 && <span className="combo-offer">Tiết kiệm {Math.round(saving / combo.originalPrice * 100)}%</span>}</Link>
    <div className="product-body">
      <span className="product-category">Combo · {combo.items.length} món</span>
      <Link to={'/combos/' + combo.id} className="product-name">{combo.name}</Link>
      <p className="text-xs text-gray-600 mb-2">{combo.items.map(item => item.productName + ' × ' + item.quantity).join(', ')}</p>
      <span className="text-gray-500 line-through text-sm">{money(combo.originalPrice)}</span>
      <strong className="product-price">{money(combo.comboPrice)}</strong>
      {saving > 0 && <span className="text-xs text-orange">Tiết kiệm {money(saving)}</span>}
      <Button disabled={combo.availableStock < 1} onClick={event => add({ kind: 'combo', id: combo.id, name: combo.name, price: combo.comboPrice, stock: combo.availableStock, imageUrl: combo.imageUrl, components: combo.items }, event.currentTarget)} className="product-add"><ShoppingCart size={17}/>{combo.availableStock ? 'Thêm combo' : 'Tạm hết hàng'}</Button>
    </div>
  </article>;
}
function Combos({ add, err }) {
  const combos = useFetch('/api/combos', err, LOAD_FAILED);
  return <><Breadcrumb current="Combo ăn vặt" /><Box>
    <div className="catalog-heading"><h1>Combo ăn vặt</h1><span>{Array.isArray(combos) ? combos.length + ' combo' : ''}</span></div>
    <p className="mb-6 text-gray-600">Chọn nhiều món yêu thích trong một combo tiết kiệm.</p>
    {combos === LOAD_FAILED ? <p>Không tải được combo. Hãy kiểm tra backend và tải lại.</p> : combos === null ? <p>Đang tải combo...</p> : combos.length ? <div className="product-grid">{combos.map(combo => <ComboCard key={combo.id} combo={combo} add={add} />)}</div> : <p>Chưa có combo đang bán.</p>}
  </Box></>;
}
function ComboDetail({ add, err }) {
  const { id } = useParams();
  const combo = useFetch('/api/combos/' + id, err, LOAD_FAILED);
  return <><Breadcrumb current={combo?.name || 'Chi tiết combo'} /><Box>
    {combo === LOAD_FAILED ? <p>Không tải được combo.</p> : combo === null ? <p>Đang tải combo...</p> : <div className="detail-layout">
      <img src={combo.imageUrl} alt={combo.name} />
      <div><span className="eyebrow">COMBO ĂN VẶT</span><h1>{combo.name}</h1><p>{combo.description}</p>
        <h2 className="font-bold mt-5 mb-2">Món trong combo</h2>
        {combo.items.map(item => <div key={item.productId} className="flex justify-between gap-3 border-b py-2 text-sm"><span>{item.productName} × {item.quantity}</span><span>{money(item.unitPrice * item.quantity)}</span></div>)}
        <p className="mt-4">Giá gốc: <span className="line-through">{money(combo.originalPrice)}</span></p>
        <strong>{money(combo.comboPrice)}</strong><p>Còn {combo.availableStock} combo có thể đặt</p>
        <Button disabled={combo.availableStock < 1} onClick={event => add({ kind: 'combo', id: combo.id, name: combo.name, price: combo.comboPrice, stock: combo.availableStock, imageUrl: combo.imageUrl, components: combo.items }, event.currentTarget)}><ShoppingCart size={18}/> Thêm combo vào giỏ</Button>
      </div>
    </div>}
  </Box></>;
}

function Home({ add, err }) {
  const categories = useFetch('/api/categories', err);
  const products = useFetch('/api/products?featured=true', err);
  const combos = useFetch('/api/combos', err);
  return <><Box><section className="home-hero"><div className="hero-content"><span className="eyebrow">SNACK CHIWIWI · GÓC ĂN VẶT</span><h1>Món ngon cho<br />mọi cuộc vui.</h1><p>Khám phá những món ăn vặt yêu thích và đặt giao tận nhà.</p><Link to="/products" className="hero-button">Khám phá sản phẩm <ChevronRight size={18} /></Link></div><div className="hero-photo-credit">Ảnh minh họa đồ ăn vặt</div></section><div className="section-heading"><div><span className="eyebrow">KHÁM PHÁ</span><h2>Danh mục sản phẩm</h2></div><Link to="/products">Xem tất cả <ChevronRight size={16}/></Link></div><div className="home-categories">{categories ? categories.map((category, index) => <Link to={'/products?categoryId=' + category.id} key={category.id}><span className="category-emoji">{['🍬','🌶️','🥤','🍿'][index % 4]}</span><strong>{category.name}</strong><ChevronRight size={17}/></Link>) : <p>Đang tải danh mục...</p>}</div><div className="section-heading"><div><span className="eyebrow">ĐƯỢC YÊU THÍCH</span><h2>Sản phẩm nổi bật</h2></div><Link to="/products">Xem tất cả <ChevronRight size={16}/></Link></div>{products ? <Grid items={products} add={add} /> : <p className="loading-products">Đang tải sản phẩm...</p>}<div className="section-heading"><div><span className="eyebrow">TIẾT KIỆM HƠN</span><h2>Combo ăn vặt</h2></div><Link to="/combos">Xem tất cả <ChevronRight size={16}/></Link></div>{combos?.length > 0 ? <div className="product-grid">{combos.slice(0,4).map(combo => <ComboCard key={combo.id} combo={combo} add={add} />)}</div> : <p className="text-gray-500">Chưa có combo đang bán.</p>}</Box></>;
}

function Products({ add, err }) {
  const [params, setParams] = useSearchParams();
  const [view, setView] = useState('grid');
  const [retry, setRetry] = useState(0);
  const categoryId = params.get('categoryId') || '';
  const q = params.get('q') || '';
  const sort = params.get('sort') || '';
  const maxPrice = params.get('maxPrice') || '';
  const categories = useFetch('/api/categories', err);
  const queryParams = new URLSearchParams();
  if (q.trim()) queryParams.set('q', q.trim());
  if (categoryId) queryParams.set('categoryId', categoryId);
  if (sort) queryParams.set('sort', sort);
  if (retry) queryParams.set('_retry', String(retry));
  const query = queryParams.toString();
  const products = useFetch('/api/products' + (query ? '?' + query : ''), err, LOAD_FAILED);
  const shown = products === LOAD_FAILED ? LOAD_FAILED : products?.filter(product => !maxPrice || product.price <= Number(maxPrice));
  const selected = categories?.find(category => String(category.id) === categoryId);
  const update = (key, value) => { const next = new URLSearchParams(params); value ? next.set(key, value) : next.delete(key); setParams(next); };
  return <><Breadcrumb current={selected?.name || 'Tất cả sản phẩm'} /><Box><div className="catalog-heading"><h1>{selected?.name || 'Tất cả sản phẩm'}</h1><span>{shown === LOAD_FAILED ? 'Lỗi tải dữ liệu' : shown ? shown.length + ' sản phẩm' : 'Đang tải...'}</span></div><div className="catalog-layout"><aside className="catalog-sidebar"><div className="filter-title">LỌC SẢN PHẨM</div><div className="filter-panel"><h2>Danh mục nhóm</h2><button className={!categoryId ? 'active' : ''} onClick={() => update('categoryId', '')}>Tất cả sản phẩm <ChevronRight size={15}/></button>{categories?.map(category => <button className={categoryId === String(category.id) ? 'active' : ''} onClick={() => update('categoryId', String(category.id))} key={category.id}>{category.name}<ChevronRight size={15}/></button>)}</div><div className="filter-panel"><h2>Giá sản phẩm</h2>{[['','Tất cả mức giá'],['30000','Dưới 30.000đ'],['50000','Dưới 50.000đ'],['100000','Dưới 100.000đ']].map(([value,label]) => <label key={value}><input type="radio" name="price" checked={maxPrice === value} onChange={() => update('maxPrice', value)}/>{label}</label>)}</div></aside><main className="catalog-main"><div className="catalog-toolbar"><div className="catalog-search"><Search size={18}/><input placeholder="Tìm món ăn vặt..." value={q} onChange={event => update('q', event.target.value)}/></div><div className="catalog-tools"><select aria-label="Sắp xếp sản phẩm" value={sort} onChange={event => update('sort', event.target.value)}><option value="">Sắp xếp mặc định</option><option value="price_asc">Giá thấp đến cao</option><option value="price_desc">Giá cao đến thấp</option></select><button className={view === 'grid' ? 'selected' : ''} onClick={() => setView('grid')} aria-label="Hiển thị lưới"><Grid2X2 size={19}/></button><button className={view === 'list' ? 'selected' : ''} onClick={() => setView('list')} aria-label="Hiển thị danh sách"><List size={20}/></button></div></div>{shown === LOAD_FAILED ? <div className="empty-products"><strong>Không tải được sản phẩm</strong><span>Kiểm tra backend rồi thử lại.</span><Button onClick={() => setRetry(value => value + 1)}>Thử lại</Button></div> : shown ? <Grid items={shown} add={add} view={view}/> : <p className="loading-products">Đang tải sản phẩm...</p>}</main></div></Box></>;
}

function Detail({ add, err }) {
  const { id } = useParams();
  const product = useFetch('/api/products/' + id, err);
  return <><Breadcrumb current={product?.name || 'Chi tiết sản phẩm'} /><Box>{product ? <div className="detail-layout"><img src={product.imageUrl} alt={product.name}/><div className="detail-copy"><span className="eyebrow">{product.categoryName}</span><h1>{product.name}</h1><strong className="detail-price">{money(product.price)}</strong><p className="detail-stock">{product.stock > 0 ? `Còn ${product.stock} sản phẩm` : 'Tạm hết hàng'}</p><section className="detail-description"><h2>Mô tả sản phẩm</h2><p>{product.description}</p><p>Hình ảnh minh họa sản phẩm đóng gói. Thông tin giá và số lượng còn được cập nhật từ cửa hàng.</p></section><Button disabled={!product.stock} onClick={event => add(product, event.currentTarget)}><ShoppingCart size={18}/> Thêm vào giỏ</Button></div></div> : <p className="loading-products">Đang tải sản phẩm...</p>}</Box></>;
}


function Cart({ cart, setCart, user, err }) {
  const [step, setStep] = useState(0);
  const [selectedKeys, setSelectedKeys] = useState([]);
  const [paymentMethod, setPaymentMethod] = useState('COD');
  const paymentOptions = useFetch('/api/payments/options', err, { qrAvailable: false });
  const [form, setForm] = useState({ fullName: user?.name || '', phone: '', address: '', note: '' });
  const [addresses, setAddresses] = useState(null);
  const [addressId, setAddressId] = useState(null);
  const [addressError, setAddressError] = useState('');
  const [checkoutNotice, setCheckoutNotice] = useState('');
  const [busy, setBusy] = useState(false);
  const navigate = useNavigate();
  const selectedItems = cart.filter(item => selectedKeys.includes(cartKey(item)));
  const selectedCount = selectedItems.reduce((sum, item) => sum + item.qty, 0);
  const total = selectedItems.reduce((sum, item) => sum + item.price * item.qty, 0);
  const selectedAddress = addresses?.find(item => item.id === addressId);
  const toggleItem = key => { setSelectedKeys(keys => keys.includes(key) ? keys.filter(item => item !== key) : [...keys, key]); setStep(0); };
  const toggleAll = () => { setSelectedKeys(selectedItems.length === cart.length ? [] : cart.map(cartKey)); setStep(0); };
  useEffect(() => {
    if (!user) return;
    let active = true;
    api.get('/api/addresses').then(response => {
      if (!active) return;
      setAddresses(response.data);
      setAddressId(response.data.find(item => item.isDefault)?.id ?? null);
    }).catch(() => { if (active) { setAddresses([]); setAddressError('Không tải được sổ địa chỉ. Bạn vẫn có thể nhập địa chỉ mới.'); } });
    return () => { active = false; };
  }, [user?.id]);
  const update = (key, qty) => { setCart(items => items.map(item =>
    cartKey(item) === key ? { ...item, qty: Math.max(1, Math.min(Number(qty) || 1, item.stock)) } : item)); setStep(0); };
  const remove = key => { setCart(items => items.filter(item => cartKey(item) !== key)); setSelectedKeys(keys => keys.filter(item => item !== key)); setStep(0); };
  async function review(event) {
    event?.preventDefault();
    if (!selectedItems.length) { err('Hãy chọn ít nhất một sản phẩm để đặt hàng.'); setStep(0); return; }
    setBusy(true); setCheckoutNotice('');
    try {
      if (addressId !== null) {
        const currentAddresses = (await api.get('/api/addresses')).data;
        if (!currentAddresses.some(item => item.id === addressId)) throw new Error('Địa chỉ đã lưu không còn tồn tại. Hãy chọn lại.');
        setAddresses(currentAddresses);
      }
      const currentCart = await Promise.all(selectedItems.map(async item => {
        const current = (await api.get(item.kind === 'combo' ? '/api/combos/' + item.id : '/api/products/' + item.id)).data;
        const stock = item.kind === 'combo' ? current.availableStock : current.stock;
        if (stock < item.qty) throw new Error(item.name + ' không đủ tồn kho.');
        return item.kind === 'combo'
          ? { ...item, name: current.name, price: current.comboPrice, stock, imageUrl: current.imageUrl, components: current.items }
          : { ...item, name: current.name, price: current.price, stock, imageUrl: current.imageUrl };
      }));
      if (currentCart.some((item, index) => item.price !== selectedItems[index].price)) setCheckoutNotice('Giá đã được cập nhật theo thông tin mới nhất. Vui lòng xem lại trước khi xác nhận.');
      const updated = new Map(currentCart.map(item => [cartKey(item), item]));
      setCart(items => items.map(item => updated.get(cartKey(item)) || item));
      setStep(2);
    } catch (error) { err(typeof error.response?.data === 'string' ? error.response.data : error.message || 'Không kiểm tra được giỏ hàng.'); }
    finally { setBusy(false); }
  }
  async function place() {
    if (!selectedItems.length) { err('Hãy chọn ít nhất một sản phẩm để đặt hàng.'); setStep(0); return; }
    setBusy(true);
    try {
      const recipient = selectedAddress ? { addressId } : { fullName: form.fullName, phone: form.phone, address: form.address };
      const response = await api.post('/api/orders', { ...recipient, note: form.note, expectedTotal: total, paymentMethod,
        items: selectedItems.filter(item => item.kind !== 'combo').map(item => ({ productId: item.id, quantity: item.qty })),
        combos: selectedItems.filter(item => item.kind === 'combo').map(item => ({ comboId: item.id, quantity: item.qty })) });
      const ordered = new Set(selectedItems.map(cartKey));
      setCart(items => items.filter(item => !ordered.has(cartKey(item))));
      setSelectedKeys([]);
      navigate(paymentMethod === 'QR' ? '/orders/' + response.data.id : '/orders');
    } catch (error) {
      err(typeof error.response?.data === 'string' ? error.response.data : error.response?.data?.detail || 'Đặt hàng thất bại. Hãy kiểm tra địa chỉ và tồn kho.');
    } finally { setBusy(false); }
  }
  return <Box><section className="cart-page">
    <div className="cart-page-heading"><div><span className="eyebrow">SNACK CHIWIWI</span><h1>Giỏ hàng của bạn</h1><p>{cart.length ? `Bạn đang có ${cart.reduce((n, item) => n + item.qty, 0)} món trong giỏ hàng.` : 'Chọn món ăn vặt yêu thích để bắt đầu.'}</p></div><Link to="/products" className="cart-continue">← Tiếp tục mua hàng</Link></div>
    {!cart.length ? <div className="cart-empty"><ShoppingCart size={48} strokeWidth={1.5}/><h2>Giỏ hàng đang trống</h2><p>Những món ngon đang chờ bạn khám phá.</p><Link to="/products" className="action-button">Khám phá sản phẩm</Link></div>
      : <div className="cart-layout">
        <div className="cart-items"><div className="cart-selection-bar"><label><input type="checkbox" checked={selectedItems.length === cart.length} onChange={toggleAll} disabled={busy} /><span>Chọn tất cả</span></label><span>Đã chọn {selectedCount} món</span></div><div className="cart-table-head"><span>Sản phẩm</span><span>Đơn giá</span><span>Số lượng</span><span>Thành tiền</span><span></span></div>{cart.map(item => <div className={'cart-row ' + (selectedKeys.includes(cartKey(item)) ? 'is-selected' : '')} key={cartKey(item)}>
          <div className="cart-row-product"><label className="cart-item-select"><input type="checkbox" checked={selectedKeys.includes(cartKey(item))} onChange={() => toggleItem(cartKey(item))} disabled={busy} /><span className="sr-only">Chọn mua {item.name}</span></label><img src={item.imageUrl} alt={item.name} /><div><strong>{item.name}</strong>{item.kind === 'combo' && <p>Combo · {item.components?.map(part => part.productName + ' × ' + part.quantity).join(', ')}</p>}</div></div>
          <span className="cart-unit-price">{money(item.price)}</span>
          <label className="cart-quantity"><span className="sr-only">Số lượng {item.name}</span><input aria-label={'Số lượng ' + item.name} type="number" min="1" max={item.stock} value={item.qty} disabled={busy} onChange={event => update(cartKey(item), event.target.value)} /></label>
          <strong className="cart-line-total">{money(item.price * item.qty)}</strong>
          <button type="button" className="cart-remove" aria-label={'Xóa ' + item.name} disabled={busy} onClick={() => remove(cartKey(item))}>Xóa</button>
        </div>)}</div>
        <aside className="cart-summary"><h2>Thông tin đơn hàng</h2><div className="cart-summary-line"><span>Tạm tính ({selectedCount} món đã chọn)</span><strong>{money(total)}</strong></div><div className="cart-summary-line"><span>Phương thức thanh toán</span><span>{step === 2 ? paymentMethod === 'QR' ? 'QR chuyển khoản' : 'COD' : 'Chọn khi xác nhận'}</span></div><div className="cart-summary-total"><span>Tổng tiền</span><strong>{money(total)}</strong></div><p className="cart-summary-hint">Phí giao hàng (nếu có) sẽ được cửa hàng xác nhận với bạn.</p>
          {step === 0 ? <><Button className="w-full mt-5" disabled={!selectedItems.length} onClick={() => user ? setStep(1) : navigate('/login')}>Đặt hàng COD</Button>{!selectedItems.length && <p className="cart-selection-note">Tích chọn món bạn muốn mua để tiếp tục.</p>}</>
            : step === 1 ? <div className="mt-5">
              <h2 className="font-bold mb-3">Thông tin nhận hàng</h2>
              {addressError && <p role="alert" className="text-red-700 text-sm mb-2">{addressError}</p>}
              {addresses?.length > 0 && <div className="space-y-2 mb-4">
                {addresses.map(address => <label key={address.id} className="block border rounded-xl p-3 cursor-pointer text-sm">
                  <input type="radio" name="shipping-address" checked={addressId === address.id} onChange={() => setAddressId(address.id)} />
                  <span className="ml-2 font-semibold">{address.fullName}</span> · {address.phone}
                  {address.isDefault && <span className="text-orange"> · Mặc định</span>}
                  <p className="ml-5">{address.addressLine}</p>
                </label>)}
              </div>}
              <label className="block mb-3 text-sm"><input type="radio" name="shipping-address" checked={addressId === null} onChange={() => setAddressId(null)} /> Nhập địa chỉ mới</label>
              {addressId === null && <form id="checkout-form" onSubmit={review}>
                <Field label="Họ tên" required minLength="2" maxLength="100" value={form.fullName} onChange={event => setForm({ ...form, fullName: event.target.value })} />
                <Field label="Số điện thoại" required pattern="[0-9+() .-]{9,20}" value={form.phone} onChange={event => setForm({ ...form, phone: event.target.value })} />
                <Field label="Địa chỉ" required minLength="5" maxLength="300" value={form.address} onChange={event => setForm({ ...form, address: event.target.value })} />
              </form>}
              <Field label="Ghi chú" value={form.note} onChange={event => setForm({ ...form, note: event.target.value })} />
              <Button className="w-full" disabled={busy} type={addressId === null ? 'submit' : 'button'} form={addressId === null ? 'checkout-form' : undefined} onClick={addressId !== null ? review : undefined}>{busy ? 'Đang kiểm tra...' : 'Xem lại đơn'}</Button>
              <Link to="/addresses" className="block text-orange mt-3 text-sm">Quản lý sổ địa chỉ</Link>
            </div>
            : <div className="mt-5">
              <h2 className="font-bold mb-3">Xác nhận đơn hàng</h2>
              {checkoutNotice && <p role="status" className="bg-orange/10 text-orange p-2 rounded-xl mb-3">{checkoutNotice}</p>}
              <p>{selectedAddress?.fullName || form.fullName} · {selectedAddress?.phone || form.phone}</p>
              <p>{selectedAddress?.addressLine || form.address}</p>
              {selectedItems.map(item => <p className="text-sm mt-2" key={cartKey(item)}>{item.name} × {item.qty} · {money(item.price * item.qty)}</p>)}
              <fieldset className="payment-choices"><legend>Phương thức thanh toán</legend>
                <label><input type="radio" name="payment-method" value="COD" checked={paymentMethod === 'COD'} onChange={() => setPaymentMethod('COD')} /><span><strong>COD</strong><small>Thanh toán khi nhận hàng</small></span></label>
                <label className={!paymentOptions?.qrAvailable ? 'is-disabled' : ''}><input type="radio" name="payment-method" value="QR" checked={paymentMethod === 'QR'} onChange={() => setPaymentMethod('QR')} disabled={!paymentOptions?.qrAvailable} /><span><strong>QR chuyển khoản</strong><small>{paymentOptions?.qrAvailable ? 'Quét QR sau khi tạo đơn; Admin đối soát thủ công' : 'Cửa hàng chưa cấu hình tài khoản nhận tiền'}</small></span></label>
              </fieldset>
              <p className="my-3 font-bold">Tổng thanh toán · {money(total)}</p>
              <Button disabled={busy} onClick={place} className="w-full">{busy ? 'Đang đặt...' : 'Xác nhận đặt hàng'}</Button>
              <button className="mt-3 text-orange" onClick={() => setStep(1)}>Chỉnh sửa</button>
            </div>}
        </aside>
      </div>}
  </section></Box>;
}

function Addresses({ err }) {
  const [addresses, setAddresses] = useState(null);
  const [editingId, setEditingId] = useState(null);
  const [form, setForm] = useState({ fullName: '', phone: '', addressLine: '' });
  const [message, setMessage] = useState('');
  const [busy, setBusy] = useState(false);
  const navigate = useNavigate();
  const load = () => api.get('/api/addresses').then(response => setAddresses(response.data)).catch(() => err('Không tải được sổ địa chỉ.'));
  useEffect(() => { if (!localStorage.getItem('token')) { navigate('/login'); return; } load(); }, []);
  const reset = () => { setEditingId(null); setForm({ fullName: '', phone: '', addressLine: '' }); };
  async function save(event) {
    event.preventDefault(); setMessage('');
    if (form.fullName.trim().length < 2 || !/^[0-9+() .-]{9,20}$/.test(form.phone.trim()) || form.addressLine.trim().length < 5) {
      setMessage('Vui lòng nhập họ tên, số điện thoại và địa chỉ hợp lệ.'); return;
    }
    setBusy(true);
    try {
      if (editingId) await api.put('/api/addresses/' + editingId, form);
      else await api.post('/api/addresses', form);
      setMessage(editingId ? 'Đã cập nhật địa chỉ.' : 'Đã thêm địa chỉ.');
      reset(); await load();
    } catch (error) { setMessage(typeof error.response?.data === 'string' ? error.response.data : 'Không lưu được địa chỉ.'); }
    finally { setBusy(false); }
  }
  async function makeDefault(id) {
    try { await api.put('/api/addresses/' + id + '/default'); setMessage('Đã đổi địa chỉ mặc định.'); await load(); }
    catch (error) { setMessage(typeof error.response?.data === 'string' ? error.response.data : 'Không đổi được địa chỉ mặc định.'); }
  }
  async function remove(id) {
    if (!window.confirm('Xóa địa chỉ này khỏi sổ địa chỉ?')) return;
    try { await api.delete('/api/addresses/' + id); setMessage('Đã xóa địa chỉ.'); if (editingId === id) reset(); await load(); }
    catch (error) { setMessage(typeof error.response?.data === 'string' ? error.response.data : 'Không xóa được địa chỉ.'); }
  }
  return <Box><h1 className="text-3xl font-black my-8">Sổ địa chỉ giao hàng</h1>
    {message && <p role="status" className="bg-orange/10 text-orange p-3 rounded-xl mb-4">{message}</p>}
    <div className="grid md:grid-cols-2 gap-6">
      <div>{addresses === null ? <p>Đang tải...</p> : addresses.length === 0 ? <p>Chưa có địa chỉ đã lưu.</p> : addresses.map(address => <div key={address.id} className="border rounded-2xl p-4 mb-3">
        <div className="flex justify-between gap-2"><b>{address.fullName}</b>{address.isDefault && <span className="text-orange text-sm">Mặc định</span>}</div>
        <p>{address.phone}</p><p>{address.addressLine}</p>
        <div className="flex flex-wrap gap-4 mt-3 text-sm">
          <button className="text-orange" onClick={() => { setEditingId(address.id); setForm({ fullName: address.fullName, phone: address.phone, addressLine: address.addressLine }); }}>Sửa</button>
          {!address.isDefault && <button className="text-orange" onClick={() => makeDefault(address.id)}>Đặt mặc định</button>}
          <button className="text-red-600" onClick={() => remove(address.id)}>Xóa</button>
        </div>
      </div>)}</div>
      <form onSubmit={save} className="bg-gray-50 p-5 rounded-2xl h-fit">
        <h2 className="font-bold text-xl mb-4">{editingId ? 'Sửa địa chỉ' : 'Thêm địa chỉ'}</h2>
        <Field label="Họ tên người nhận" required minLength="2" maxLength="100" value={form.fullName} onChange={event => setForm({ ...form, fullName: event.target.value })} />
        <Field label="Số điện thoại" required pattern="[0-9+() .-]{9,20}" value={form.phone} onChange={event => setForm({ ...form, phone: event.target.value })} />
        <Field label="Địa chỉ giao hàng" required minLength="5" maxLength="300" value={form.addressLine} onChange={event => setForm({ ...form, addressLine: event.target.value })} />
        <Button disabled={busy}>{busy ? 'Đang lưu...' : editingId ? 'Lưu thay đổi' : 'Thêm địa chỉ'}</Button>
        {editingId && <button type="button" className="ml-4" onClick={reset}>Hủy sửa</button>}
      </form>
    </div>
  </Box>;
}

function Auth({setUser,err}){const[register,setRegister]=useState(false),[f,setF]=useState({name:'',email:'',password:''}),nav=useNavigate();async function submit(e){e.preventDefault();try{if(register){await api.post('/api/auth/register',f);setRegister(false)}else{const r=await api.post('/api/auth/login',f);localStorage.setItem('token',r.data.token);localStorage.setItem('user',JSON.stringify(r.data.user));setUser(r.data.user);nav('/')}}catch(e){err(e.response?.data?.title||e.response?.data||'Đăng nhập thất bại')}}return <Box><div className="max-w-md mx-auto bg-white rounded-3xl p-8 mt-10"><h1 className="text-3xl font-black mb-6">{register?'Đăng ký':'Đăng nhập'}</h1><form onSubmit={submit}>{register&&<Field label="Họ tên" required value={f.name} onChange={e=>setF({...f,name:e.target.value})}/>}<Field label="Email" type="email" required value={f.email} onChange={e=>setF({...f,email:e.target.value})}/><Field label="Mật khẩu" type="password" minLength={register?8:1} required value={f.password} onChange={e=>setF({...f,password:e.target.value})}/><Button className="w-full">{register?'Tạo tài khoản':'Đăng nhập'}</Button></form><button className="mt-4 text-orange" onClick={()=>setRegister(!register)}>{register?'Đã có tài khoản? Đăng nhập':'Chưa có tài khoản? Đăng ký'}</button></div></Box>}
function Orders({ err }) {
  const [refresh, setRefresh] = useState(0);
  const [cancelId, setCancelId] = useState(null);
  const [reason, setReason] = useState('');
  const [busyId, setBusyId] = useState(null);
  const [message, setMessage] = useState('');
  const orders = useFetch('/api/orders?refresh=' + refresh, err, LOAD_FAILED);
  async function cancel(orderId) {
    if (reason.trim().length < 5) { setMessage('Vui lòng nhập lý do hủy ít nhất 5 ký tự.'); return; }
    if (!window.confirm('Bạn chắc chắn muốn hủy đơn #' + orderId + '?')) return;
    setBusyId(orderId); setMessage('');
    try {
      await api.post('/api/orders/' + orderId + '/cancel', { reason: reason.trim() });
      setCancelId(null); setReason('');
      setMessage('Đã hủy đơn #' + orderId + ' và hoàn lại tồn kho.');
      setRefresh(value => value + 1);
    } catch (error) {
      setMessage(typeof error.response?.data === 'string' ? error.response.data : 'Không thể hủy đơn. Hãy tải lại để kiểm tra trạng thái.');
    } finally { setBusyId(null); }
  }
  return <Box>
    <h1 className="text-3xl font-black my-8">Đơn hàng của tôi</h1>
    {message && <p role="status" className="bg-orange/10 text-orange p-3 rounded-xl mb-4">{message}</p>}
    {orders === LOAD_FAILED ? <div>Không tải được lịch sử đơn. <Button onClick={() => setRefresh(value => value + 1)}>Thử lại</Button></div>
      : orders === null ? <p>Đang tải...</p>
      : orders.length === 0 ? <p>Chưa có đơn hàng.</p>
      : orders.map(order => <div className="bg-white rounded-2xl p-5 mb-4 border border-gray-100" key={order.id}>
        <div className="flex flex-wrap justify-between gap-2"><b>Đơn #{order.id}</b><span className="text-orange font-bold">{order.status}</span></div>
        <p className="payment-summary">{order.paymentMethod === 'QR' ? 'QR chuyển khoản' : 'COD'} · <strong>{order.paymentStatus}</strong></p>
        <p className="text-sm text-gray-500">{new Date(order.createdAt).toLocaleString('vi-VN')}</p>
        <p className="text-sm">{order.fullName} · {order.phone} · {order.address}</p>
        {order.items.map((item, index) => <div key={index} className="mt-2">
          <p>{item.comboId ? 'Combo: ' : ''}{item.productName} × {item.quantity} · {money(item.unitPrice)}</p>
          {item.comboId && <div className="pl-4 text-sm text-gray-600">
            <p>Giá gốc lúc đặt: {money(item.originalUnitPrice)}</p>
            {item.components?.map(part => <p key={part.productId}>↳ {part.productName} × {part.quantityPerCombo} mỗi combo · {money(part.unitPrice)}/món</p>)}
          </div>}
        </div>)}
        <b>{money(order.total)}</b>
        <Link to={'/orders/' + order.id} className="order-detail-link">Xem chi tiết đơn{order.paymentMethod === 'QR' && order.paymentStatus === 'Chờ thanh toán' ? ' và mã QR' : ''} →</Link>
        {order.cancelReason && <p className="text-sm text-gray-600 mt-2">Lý do hủy: {order.cancelReason}</p>}
        {order.status === 'Chờ xác nhận' && (cancelId === order.id
          ? <div className="mt-4 max-w-lg"><label className="block font-semibold mb-2" htmlFor={'reason-' + order.id}>Lý do hủy đơn</label>
              <textarea id={'reason-' + order.id} className="w-full border rounded-xl p-3" rows="2" maxLength="300" value={reason} onChange={event => setReason(event.target.value)} placeholder="Ví dụ: Tôi muốn thay đổi sản phẩm trong đơn" />
              <div className="flex gap-3 mt-2"><Button disabled={busyId === order.id} onClick={() => cancel(order.id)}>{busyId === order.id ? 'Đang hủy...' : 'Xác nhận hủy'}</Button><button onClick={() => { setCancelId(null); setReason(''); }}>Đóng</button></div>
            </div>
          : <button className="text-orange font-semibold mt-3 block" onClick={() => { setCancelId(order.id); setMessage(''); }}>Hủy đơn</button>)}
      </div>)}
  </Box>;
}

function Admin({err}){const[refresh,setRefresh]=useState(0),[tab,setTab]=useState('dashboard'),[edit,setEdit]=useState(null),[name,setName]=useState('');const dash=useFetch('/api/admin/dashboard?x='+refresh,err),orders=useFetch('/api/admin/orders?x='+refresh,err),products=useFetch('/api/admin/products?x='+refresh,err),cats=useFetch('/api/categories',err);async function save(e){e.preventDefault();try{await api[edit.id?'put':'post']('/api/admin/products'+(edit.id?'/'+edit.id:''),edit);setEdit(null);setRefresh(x=>x+1)}catch(e){err(e.response?.data||'Không lưu được')}}async function status(id,s){const reason=s==='Đã hủy'?window.prompt('Nhập lý do hủy đơn (ít nhất 5 ký tự):'):null;if(s==='Đã hủy'&&(reason===null||reason.trim().length<5)){if(reason!==null)err('Lý do hủy cần ít nhất 5 ký tự.');return}try{await api.put('/api/admin/orders/'+id+'/status',{status:s,reason});setRefresh(x=>x+1)}catch(e){err(e.response?.data||'Không cập nhật được')}}async function addCat(e){e.preventDefault();try{await api.post('/api/admin/categories',{name});setName('');location.reload()}catch(e){err(e.response?.data||'Không thêm được danh mục')}}return <Box><h1 className="text-3xl font-black my-8">Quản trị cửa hàng</h1><div className="flex gap-3 mb-6">{['dashboard','products','categories','combos','orders'].map(t=><button key={t} className={'rounded-full px-4 py-2 '+(tab===t?'bg-orange text-white':'bg-white')} onClick={()=>setTab(t)}>{({dashboard:'Tổng quan',products:'Sản phẩm',categories:'Danh mục',combos:'Combo',orders:'Đơn hàng'})[t]}</button>)}</div>{tab==='dashboard'&&<div className="grid md:grid-cols-3 gap-4">{dash?<> <div className="bg-white p-6 rounded-2xl">Số đơn<br/><b className="text-3xl">{dash.orders}</b></div><div className="bg-white p-6 rounded-2xl">Doanh thu hoàn thành<br/><b className="text-3xl">{money(dash.revenue)}</b></div><div className="bg-white p-6 rounded-2xl">Sắp hết hàng{dash.lowStock.map(x=><p key={x.id}>{x.name}: {x.stock}</p>)}</div></>:'Đang tải...'}</div>}{tab==='categories'&&<><form onSubmit={addCat} className="flex gap-2 mb-5"><input className="bg-white p-2 rounded" placeholder="Danh mục mới" required value={name} onChange={e=>setName(e.target.value)}/><Button>Thêm</Button></form>{cats?.map(c=><CategoryRow key={c.id} c={c} err={err}/>)}</>}{tab==='products'&&<><Button onClick={()=>setEdit({name:'',description:'',price:0,stock:0,imageUrl:'',categoryId:cats?.[0]?.id||1,isFeatured:false,isVisible:true})}>Thêm sản phẩm</Button>{edit&&<form onSubmit={save} className="bg-white rounded-2xl p-5 my-4 grid md:grid-cols-2 gap-3">{['name','description','price','stock','imageUrl'].map(k=><Field key={k} label={k} required type={['price','stock'].includes(k)?'number':'text'} value={edit[k]} onChange={e=>setEdit({...edit,[k]:['price','stock'].includes(k)?Number(e.target.value):e.target.value})}/>)}<select value={edit.categoryId} onChange={e=>setEdit({...edit,categoryId:Number(e.target.value)})}>{cats?.map(c=><option key={c.id} value={c.id}>{c.name}</option>)}</select><label><input type="checkbox" checked={edit.isFeatured} onChange={e=>setEdit({...edit,isFeatured:e.target.checked})}/> Nổi bật</label><label><input type="checkbox" checked={edit.isVisible} onChange={e=>setEdit({...edit,isVisible:e.target.checked})}/> Hiển thị</label><div><Button>Lưu</Button> <button type="button" onClick={()=>setEdit(null)}>Hủy</button></div></form>}{products?.map(p=><div key={p.id} className="bg-white p-4 rounded-xl flex justify-between my-2"><span>{p.name} · {money(p.price)} · Tồn {p.stock} {p.isVisible?'':'· Đã ẩn'}</span><button className="text-orange" onClick={()=>setEdit(p)}>Sửa</button></div>)}</>}{tab==='orders'&&<AdminOrders orders={orders} status={status}/>}{tab==='combos'&&<AdminCombos err={err}/>}</Box>}
function PaymentQr({ order }) {
  const [imageFailed, setImageFailed] = useState(false);
  if (!order.paymentQrUrl) return null;
  return <section className="payment-qr" aria-label="Mã QR chuyển khoản">
    <h2>Quét QR để chuyển khoản</h2>
    <p>Kiểm tra đúng tài khoản nhận, số tiền và nội dung trước khi xác nhận trong ứng dụng ngân hàng.</p>
    {!imageFailed ? <img src={order.paymentQrUrl} alt={'QR chuyển khoản đơn ' + order.id} onError={() => setImageFailed(true)} />
      : <p role="alert">Không tải được ảnh QR. Bạn vẫn có thể chuyển khoản theo thông tin bên dưới.</p>}
    <dl><div><dt>Ngân hàng</dt><dd>{order.paymentBankId}</dd></div><div><dt>Số tài khoản</dt><dd>{order.paymentAccountNumber}</dd></div><div><dt>Chủ tài khoản</dt><dd>{order.paymentAccountName}</dd></div><div><dt>Số tiền</dt><dd>{money(order.total)}</dd></div><div><dt>Nội dung chuyển khoản</dt><dd>{order.transferReference}</dd></div></dl>
    <p className="payment-qr-note">Thông báo chuyển khoản không xác nhận tiền đã vào tài khoản. Cửa hàng sẽ đối soát và cập nhật trạng thái.</p>
  </section>;
}

function OrderLines({ items }) {
  return <div className="order-lines">{items.map((item, index) => <div key={index}>
    <strong>{item.comboId ? 'Combo: ' : ''}{item.productName} × {item.quantity}</strong><span>{money(item.unitPrice * item.quantity)}</span>
    {item.comboId && <div className="order-components">{item.components?.map(part => <p key={part.productId}>↳ {part.productName} × {part.quantityPerCombo} mỗi combo · {money(part.unitPrice)}/món</p>)}</div>}
  </div>)}</div>;
}

function OrderDetail({ err }) {
  const { id } = useParams();
  const [refresh, setRefresh] = useState(0);
  const [busy, setBusy] = useState(false);
  const [message, setMessage] = useState('');
  const order = useFetch('/api/orders/' + id + '?refresh=' + refresh, err, LOAD_FAILED);
  async function reportTransfer() {
    if (!window.confirm('Gửi thông báo đã chuyển khoản? Trạng thái chỉ đổi sau khi Admin kiểm tra giao dịch.')) return;
    setBusy(true); setMessage('');
    try { await api.post('/api/orders/' + id + '/transfer-notice'); setMessage('Đã gửi thông báo. Vui lòng chờ cửa hàng đối soát.'); setRefresh(value => value + 1); }
    catch (error) { setMessage(typeof error.response?.data === 'string' ? error.response.data : 'Không gửi được thông báo. Hãy tải lại đơn.'); }
    finally { setBusy(false); }
  }
  return <Box><div className="order-detail-page"><Link to="/orders" className="order-detail-link">← Đơn hàng của tôi</Link>
    {order === LOAD_FAILED ? <p>Không tải được đơn hàng hoặc bạn không có quyền xem đơn này.</p> : order === null ? <p>Đang tải đơn hàng...</p>
      : <><h1>Đơn #{order.id}</h1><div className="order-detail-grid"><section className="order-detail-panel">
        <p><strong>Trạng thái đơn:</strong> {order.status}</p><p><strong>Thanh toán:</strong> {order.paymentMethod === 'QR' ? 'QR chuyển khoản' : 'COD'} · {order.paymentStatus}</p>
        <p><strong>Người nhận:</strong> {order.fullName} · {order.phone}</p><p><strong>Địa chỉ:</strong> {order.address}</p>{order.note && <p><strong>Ghi chú:</strong> {order.note}</p>}
        <h2>Sản phẩm đã đặt</h2><OrderLines items={order.items} /><p className="order-detail-total">Tổng tiền: {money(order.total)}</p>
        {order.cancelReason && <p>Lý do hủy: {order.cancelReason}</p>}
        {order.paymentConfirmedAt && <p>Đã xác nhận thanh toán lúc {new Date(order.paymentConfirmedAt).toLocaleString('vi-VN')}.</p>}
        {order.status === 'Đã hủy' && order.paymentStatus === 'Đã thanh toán' && <p className="payment-message">Đơn đã hủy sau khi thanh toán. Vui lòng liên hệ cửa hàng để được xử lý hoàn tiền.</p>}
      </section><div>{order.paymentQrUrl && <><PaymentQr order={order}/>{message && <p role="status" className="payment-message">{message}</p>}
        {order.transferReportedAt ? <p className="payment-message">Đã báo chuyển khoản lúc {new Date(order.transferReportedAt).toLocaleString('vi-VN')}; đang chờ Admin đối soát.</p>
          : <Button disabled={busy} onClick={reportTransfer} className="w-full mt-4">{busy ? 'Đang gửi...' : 'Tôi đã chuyển khoản'}</Button>}</>}
      </div></div></>}
  </div></Box>;
}

function AdminOrderDetail({ err }) {
  const { id } = useParams();
  const [refresh, setRefresh] = useState(0);
  const [busy, setBusy] = useState(false);
  const [message, setMessage] = useState('');
  const order = useFetch('/api/admin/orders/' + id + '?refresh=' + refresh, err, LOAD_FAILED);
  async function confirmPayment() {
    if (!window.confirm(order?.paymentMethod === 'QR'
      ? 'Bạn đã kiểm tra tài khoản ngân hàng, đúng số tiền và nội dung chuyển khoản của đơn này?'
      : 'Bạn đã nhận đủ tiền COD của đơn này?')) return;
    setBusy(true); setMessage('');
    try { await api.post('/api/admin/orders/' + id + '/confirm-payment'); setMessage('Đã xác nhận thanh toán.'); setRefresh(value => value + 1); }
    catch (error) { setMessage(typeof error.response?.data === 'string' ? error.response.data : 'Không xác nhận được thanh toán. Hãy tải lại đơn.'); }
    finally { setBusy(false); }
  }
  return <Box><div className="order-detail-page"><Link to="/admin" className="order-detail-link">← Trang quản trị</Link>
    {order === LOAD_FAILED ? <p>Không tải được đơn hàng.</p> : order === null ? <p>Đang tải đơn hàng...</p>
      : <><h1>Đơn #{order.id}</h1><div className="order-detail-grid"><section className="order-detail-panel">
        <p><strong>Trạng thái đơn:</strong> {order.status}</p><p><strong>Phương thức:</strong> {order.paymentMethod === 'QR' ? 'QR chuyển khoản' : 'COD'}</p>
        <p><strong>Trạng thái thanh toán:</strong> {order.paymentStatus}</p><p><strong>Người nhận:</strong> {order.fullName} · {order.phone}</p><p><strong>Địa chỉ:</strong> {order.address}</p>{order.note && <p><strong>Ghi chú:</strong> {order.note}</p>}
        <h2>Chi tiết đơn</h2><OrderLines items={order.items} /><p className="order-detail-total">Tổng tiền: {money(order.total)}</p>
        {order.cancelReason && <p>Lý do hủy: {order.cancelReason}</p>}
        {order.status === 'Đã hủy' && order.paymentStatus === 'Đã thanh toán' && <p className="payment-message">Cần xử lý hoàn tiền thủ công cho đơn đã thanh toán.</p>}
      </section><section className="order-detail-panel"><h2>Đối soát thanh toán</h2>
        {order.paymentMethod === 'QR' && <><p><strong>Ngân hàng:</strong> {order.paymentBankId}</p><p><strong>Số tài khoản:</strong> {order.paymentAccountNumber}</p><p><strong>Chủ tài khoản:</strong> {order.paymentAccountName}</p><p><strong>Nội dung:</strong> {order.transferReference}</p><p><strong>Số tiền cần đối chiếu:</strong> {money(order.total)}</p><p><strong>Khách báo chuyển khoản:</strong> {order.transferReportedAt ? new Date(order.transferReportedAt).toLocaleString('vi-VN') : 'Chưa báo'}</p></>}
        {order.paymentConfirmedAt && <p><strong>Đã xác nhận:</strong> {new Date(order.paymentConfirmedAt).toLocaleString('vi-VN')} bởi {order.paymentConfirmedByEmail || 'Admin #' + order.paymentConfirmedByUserId}</p>}
        {message && <p role="status" className="payment-message">{message}</p>}
        {order.paymentStatus !== 'Đã thanh toán' && order.status !== 'Đã hủy' && <Button disabled={busy} onClick={confirmPayment} className="mt-4">{busy ? 'Đang xác nhận...' : 'Xác nhận đã thanh toán'}</Button>}
      </section></div></>}
  </div></Box>;
}

function AdminOrders({ orders, status }) {
  if (!orders) return <p>Đang tải đơn hàng...</p>;
  if (!orders.length) return <p>Chưa có đơn hàng.</p>;
  return orders.map(order => <article key={order.id} className="admin-order-card">
    <div className="admin-order-head"><div><strong>Đơn #{order.id} · {money(order.total)}</strong><p>{order.fullName} · {order.phone}</p></div><Link to={'/admin/orders/' + order.id} className="order-detail-link">Xem chi tiết và đối soát →</Link></div>
    <p><strong>Trạng thái đơn:</strong> {order.status}</p><p><strong>Thanh toán:</strong> {order.paymentMethod === 'QR' ? 'QR chuyển khoản' : 'COD'} · {order.paymentStatus}</p>
    {order.transferReportedAt && order.paymentStatus === 'Chờ thanh toán' && <p className="payment-message">Khách báo đã chuyển khoản; cần kiểm tra giao dịch.</p>}
    {order.paymentConfirmedAt && <p className="text-sm">Xác nhận bởi {order.paymentConfirmedByEmail || 'Admin #' + order.paymentConfirmedByUserId} lúc {new Date(order.paymentConfirmedAt).toLocaleString('vi-VN')}</p>}
    {order.cancelReason && <p className="text-sm text-red-700">Lý do hủy: {order.cancelReason}</p>}
    <select className="border rounded p-2 mt-2" value={order.status} disabled={['Hoàn thành','Đã hủy'].includes(order.status)} onChange={event => status(order.id, event.target.value)}>{['Chờ xác nhận','Đã xác nhận','Đang giao','Hoàn thành','Đã hủy'].map(value => <option key={value}>{value}</option>)}</select>
  </article>);
}

function AdminCombos({ err }) {
  const [refresh, setRefresh] = useState(0);
  const combos = useFetch('/api/admin/combos?x=' + refresh, err, LOAD_FAILED);
  const products = useFetch('/api/admin/products?x=' + refresh, err, []);
  const [editing, setEditing] = useState(null);
  const [busy, setBusy] = useState(false);
  const [message, setMessage] = useState('');
  const empty = () => ({ name: '', description: '', imageUrl: '', comboPrice: 0, isVisible: true, items: [] });
  const startEdit = combo => setEditing({ id: combo.id, name: combo.name, description: combo.description,
    imageUrl: combo.imageUrl, comboPrice: combo.comboPrice, isVisible: combo.isVisible,
    items: combo.items.map(part => ({ productId: part.productId, quantity: part.quantity })) });
  const setPart = (index, changes) => setEditing(value => ({ ...value, items: value.items.map((part, i) => i === index ? { ...part, ...changes } : part) }));
  async function save(event) {
    event.preventDefault(); setMessage('');
    if (!editing.items.length) { setMessage('Hãy chọn ít nhất một món trong combo.'); return; }
    setBusy(true);
    try {
      await api[editing.id ? 'put' : 'post']('/api/admin/combos' + (editing.id ? '/' + editing.id : ''), editing);
      setMessage(editing.id ? 'Đã cập nhật combo.' : 'Đã tạo combo.');
      setEditing(null); setRefresh(value => value + 1);
    } catch (error) { setMessage(typeof error.response?.data === 'string' ? error.response.data : 'Không lưu được combo.'); }
    finally { setBusy(false); }
  }
  async function toggle(combo) {
    if (!window.confirm((combo.isVisible ? 'Ẩn' : 'Hiển thị') + ' combo ' + combo.name + '?')) return;
    try {
      await api.put('/api/admin/combos/' + combo.id, { name: combo.name, description: combo.description,
        imageUrl: combo.imageUrl, comboPrice: combo.comboPrice, isVisible: !combo.isVisible,
        items: combo.items.map(part => ({ productId: part.productId, quantity: part.quantity })) });
      setMessage(combo.isVisible ? 'Đã ẩn combo.' : 'Đã hiển thị combo.');
      setRefresh(value => value + 1);
    } catch (error) { setMessage(typeof error.response?.data === 'string' ? error.response.data : 'Không đổi được trạng thái combo.'); }
  }
  return <div>
    <div className="flex justify-between items-center mb-4"><h2 className="text-xl font-bold">Quản lý combo</h2><Button onClick={() => setEditing(empty())}>Tạo combo</Button></div>
    {message && <p role="status" className="bg-orange/10 text-orange p-3 rounded-xl mb-4">{message}</p>}
    {editing && <form onSubmit={save} className="border rounded-2xl p-5 mb-5 max-w-2xl">
      <h3 className="font-bold mb-3">{editing.id ? 'Sửa combo' : 'Tạo combo mới'}</h3>
      <Field label="Tên combo" required minLength="2" maxLength="150" value={editing.name} onChange={event => setEditing({ ...editing, name: event.target.value })} />
      <label className="block mb-3"><span className="font-semibold block mb-1">Mô tả</span><textarea required minLength="5" maxLength="1000" className="w-full border rounded-xl p-3" value={editing.description} onChange={event => setEditing({ ...editing, description: event.target.value })} /></label>
      <Field label="Đường dẫn ảnh combo" required value={editing.imageUrl} onChange={event => setEditing({ ...editing, imageUrl: event.target.value })} />
      <p className="text-xs text-gray-500 mb-3">Có thể dùng đường dẫn ảnh sản phẩm, ví dụ /images/products/rice-paper-salad.jpg.</p>
      <Field label="Giá combo" type="number" min="1" required value={editing.comboPrice} onChange={event => setEditing({ ...editing, comboPrice: Number(event.target.value) })} />
      <label className="block mb-4"><input type="checkbox" checked={editing.isVisible} onChange={event => setEditing({ ...editing, isVisible: event.target.checked })} /> Hiển thị cho khách</label>
      <h4 className="font-bold mb-2">Các món trong combo</h4>
      {editing.items.map((part, index) => <div key={index} className="flex gap-2 mb-2">
        <select className="border rounded-xl p-2 flex-1 min-w-0" value={part.productId} onChange={event => setPart(index, { productId: Number(event.target.value) })}>
          {products?.map(product => <option key={product.id} value={product.id}>{product.name}</option>)}
        </select>
        <input type="number" aria-label="Số lượng món trong combo" min="1" max="100" className="border rounded-xl p-2 w-20" value={part.quantity} onChange={event => setPart(index, { quantity: Number(event.target.value) })} />
        <button type="button" className="text-red-600" onClick={() => setEditing(value => ({ ...value, items: value.items.filter((_, i) => i !== index) }))}>Xóa</button>
      </div>)}
      <button type="button" className="text-orange font-semibold block my-3" disabled={!products?.length} onClick={() => setEditing(value => ({ ...value, items: [...value.items, { productId: products.find(product => !value.items.some(part => part.productId === product.id))?.id || products[0].id, quantity: 1 }] }))}>+ Thêm món</button>
      <Button disabled={busy}>{busy ? 'Đang lưu...' : 'Lưu combo'}</Button>
      <button type="button" className="ml-4" onClick={() => setEditing(null)}>Hủy</button>
    </form>}
    {combos === LOAD_FAILED ? <p>Không tải được danh sách combo.</p> : combos === null ? <p>Đang tải...</p> : combos.length === 0 ? <p>Chưa có combo.</p> : combos.map(combo => <div key={combo.id} className="border rounded-2xl p-4 mb-3 flex flex-wrap justify-between gap-3">
      <div><b>{combo.name}</b><p className="text-sm">{combo.items.map(part => part.productName + ' × ' + part.quantity).join(', ')}</p><p>{money(combo.comboPrice)} · Giá gốc {money(combo.originalPrice)} · Còn {combo.availableStock} combo {combo.isVisible ? '' : '· Đã ẩn'}</p></div>
      <div className="flex gap-3"><button className="text-orange" onClick={() => startEdit(combo)}>Sửa</button><button className="text-orange" onClick={() => toggle(combo)}>{combo.isVisible ? 'Ẩn' : 'Hiển thị'}</button></div>
    </div>)}
  </div>;
}

function CategoryRow({c,err}){const[name,setName]=useState(c.name);return <form className="bg-white p-3 rounded-xl mb-2 flex gap-2" onSubmit={async e=>{e.preventDefault();try{await api.put('/api/admin/categories/'+c.id,{name})}catch(x){err(x.response?.data||'Không lưu được')}}}><input className="flex-1" value={name} onChange={e=>setName(e.target.value)}/><Button>Lưu</Button></form>}
createRoot(document.getElementById('root')).render(<BrowserRouter><App/></BrowserRouter>);

