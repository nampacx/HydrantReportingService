import { Address } from "./address";

export interface Hydrant {
    address: Address;
    approved: boolean;
    id: string;
    type: string;
}
