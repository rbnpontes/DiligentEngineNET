function module_extensions_setup(module) {
    const delegate_tbl = {};
    const obj_list = [];

    function memory_copy_net_2_diligent(mem_ptr, net_buffer, size) {
        const ptr_idx = mem_ptr >> 0;
        /**
         * @type {Uint8Array}
         */
        const buffer = module.HEAPU8.subarray(ptr_idx, ptr_idx + size);
        net_buffer.copyTo(buffer, 0);
    }

    function memory_copy_diligent_2_net(mem_ptr, net_buffer, size) {
        const ptr_idx = mem_ptr >> 0;
        /**
         * @type {Uint8Array}
         */
        const buffer = module.HEAPU8.subarray(ptr_idx, ptr_idx + size);
        net_buffer.set(buffer, 0);
    }

    function delegate_register(delegate, signature) {
        const delegate_entry = { 
            call: delegate, 
            ptr: 0,
            signature,
        };
        const delegate_ptr = module.addFunction((...args)=> {
            const target_delegate = delegate_tbl[delegate_entry.ptr];
            if(!target_delegate)
                return;

            const obj_idx = object_list__push(args);
            target_delegate.call(obj_idx);
            object_list__free(obj_idx);
        }, signature);
        delegate_entry.ptr = delegate_ptr;
        delegate_tbl[delegate_ptr] = delegate_entry;
        return delegate_ptr;
    }

    function delegate_override(delegate_ptr, delegate) {
        if(!delegate_tbl[delegate_ptr])
            return;
        const delegate_entry = delegate_tbl[delegate_ptr];
        delegate_entry.call.dispose();
        delegate_entry.call = delegate;
    }

    function delegate_unregister(delegate_ptr) {
        if(!delegate_tbl[delegate_ptr])
            return;
        const entry = delegate_tbl[delegate_ptr];
        delete delegate_tbl[delegate_ptr];
        module.removeFunction(delegate_ptr);
        entry.call.dispose();
    }

    function object_list__push(obj) {
        obj_list.push(obj);
        return obj_list.length - 1;
    }

    function object_list__get(obj_idx) {
        return obj_list[obj_idx];
    }

    function object_list__free(obj_idx) {
        if(!obj_list[obj_idx])
            return;
        obj_list.splice(obj_idx, 1);
    }

    function object_list__array_get(obj_idx, idx) {
        if(!obj_list[obj_idx])
            return;
        return obj_list[obj_idx][idx];
    }

    function object_list__array_set(obj_idx, idx, value) {
        if(!obj_list[obj_idx])
            return;
        obj_list[obj_idx][idx] = value;
    }

    const calls = {
        memory_copy_diligent_2_net,
        memory_copy_net_2_diligent,
        delegate_register,
        delegate_override,
        delegate_unregister,
        object_list__get,
        object_list__push,
        object_list__free,
        object_list__array_get,
        object_list__array_set
    };

    Object.entries(calls).forEach(([key, value]) => {
        module[key] = value;
    });
}
module_extensions_setup(Module);