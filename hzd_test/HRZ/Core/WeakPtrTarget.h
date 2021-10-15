#pragma once

#include "../PCore/Util.h"

#include "RTTI.h"

namespace HRZ
{

DECL_RTTI(WeakPtrTarget);

class WeakPtrTarget
{
public:
	TYPE_RTTI(WeakPtrTarget);

	void *m_UnknownList = nullptr;

	virtual const RTTI *GetRTTI() const;	// 0
	virtual ~WeakPtrTarget();				// 1
};
assert_size(WeakPtrTarget, 0x10);

}