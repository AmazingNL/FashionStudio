// Mirrors the backend's DTOs exactly (FashionStudio.Api/DTOs) — field names
// and casing must match what the server actually serializes (camelCase).
// Only the DTOs the mobile app currently uses are ported here; add more as
// screens need them rather than porting the whole backend speculatively.

export type OrderStatus = 'New' | 'InProgress' | 'Fitting' | 'Ready' | 'Delivered' | 'Cancelled' | 'Rejected';
export type FittingOutcome = 'Pending' | 'Approved' | 'Rejected';
export type PaymentMethod = 'Cash' | 'CreditCard' | 'MobilePayment' | 'BankTransfer' | 'Other';
export type CurrencyCode = 'NGN' | 'USD' | 'GBP' | 'EUR';
export type WorkSpaceRole = 'Owner' | 'Assistant' | 'Tailor';

export interface PageResultDTO<T> {
  items: T[] | null;
  totalCount: number;
  pageNumber: number;
  pageSize: number;
  totalPages: number;
  hasPreviousPage: boolean;
  hasNextPage: boolean;
}

export interface QueryParam {
  pageNumber?: number;
  pageSize?: number;
  searchTerm?: string;
  sortBy?: string;
  isDescending?: boolean;
}

// — Auth / User —
export interface LoginRequestDTO {
  userName: string;
  password: string;
}

export interface RegisterRequestDTO {
  firstName: string;
  lastName: string;
  userName: string;
  email: string;
  password: string;
}

export interface UserResponseDTO {
  id: number;
  workSpace: string;
  firstName: string;
  lastName: string;
  fullName: string;
  userName: string;
  email: string;
  role: string;
  joinedAt: string;
}

// — WorkSpace —
export interface WorkSpaceMemberDTO {
  userId: number;
  fullName: string;
  role: WorkSpaceRole | null;
  joinedAt: string;
}

export interface WorkSpaceResponseDTO {
  id: number;
  name: string;
  description: string;
  currency: string;
  createdAt: string;
  defaultMeasurementUnit: 'Cm' | 'Inch';
  memberships: WorkSpaceMemberDTO[];
  customers: unknown[];
}

// — Customer —
export interface CustomerRequestDTO {
  fullName: string;
  phone: string;
  email?: string;
  address?: string;
  preferredContactMethod?: string;
  socialHandle?: string;
  birthday?: string;
  notes?: string;
}

export interface CustomerResponseDTO {
  id: number;
  fullName: string;
  phone: string;
  email: string;
  address: string;
  preferredContactMethod: string;
  socialHandle: string;
  birthday: string;
  notes: string;
  isActive: boolean;
  workSpaceId: number | null;
  createdAt: string;
  updatedAt: string;
}

// — Order —
export interface OrderRequestDTO {
  customerId: number;
  workSpaceId: number;
  assignedToUserId?: number | null;
  title: string;
  description?: string;
  quotedPrice: number;
  discount?: number;
  currency?: CurrencyCode;
  deadlineDate: string;
  eventDate: string;
}

export interface OrderResponseDTO {
  id: number;
  customerId: number;
  workSpaceId: number;
  assignedToUserId: number | null;
  createdByUserId: number;
  title: string;
  description: string;
  status: OrderStatus;
  quotedPrice: number;
  discount: number;
  currency: CurrencyCode;
  deadlineDate: string;
  eventDate: string;
  deliveredDate: string | null;
  createdAt: string;
  updatedAt: string;
}

export interface OrderUpdateDTO {
  title?: string;
  description?: string;
  status?: OrderStatus;
  quotedPrice?: number;
  discount?: number;
  currency?: CurrencyCode;
  deadlineDate?: string;
  eventDate?: string;
  deliveredDate?: string;
}

// — Fitting —
export interface FittingRequestDTO {
  orderId: number;
  fittingDate: string;
  notes?: string;
}

export interface FittingUpdateDTO {
  fittingDate?: string;
  notes?: string;
  outcome?: FittingOutcome;
}

export interface FittingResponseDTO {
  id: number;
  orderId: number;
  workSpaceId: number;
  customerId: number;
  createdByUserId: number;
  fittingDate: string;
  notes: string;
  outcome: FittingOutcome;
  createdAt: string;
  updatedAt: string;
}

// — Payment —
export interface PaymentRequestDTO {
  orderId: number;
  amount: number;
  method?: PaymentMethod;
  paymentDate: string;
  notes?: string;
  receiptReference?: string;
}

export interface PaymentResponseDTO {
  id: number;
  orderId: number;
  workSpaceId: number;
  createdByUserId: number;
  amount: number;
  method: PaymentMethod;
  paymentDate: string;
  notes: string;
  receiptReference: string;
  createdAt: string;
}
